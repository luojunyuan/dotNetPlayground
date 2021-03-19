using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace MicrosoftSqlitePractice
{
    public class EhServerTransitionDateTime
    {
        public void Games()
        {
            using var connection = new SqliteConnection("DataSource=C:\\Users\\k1mlka\\Downloads\\ehserver.sqlite");
            connection.Open();

            var query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, update_timestamp FROM dle_vnr_gamefiles
                ";
            //query.Parameters.AddWithValue("$id", id);
            using var reader = query.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetString(0);
                var unixTime = reader.GetString(1);

                var update = connection.CreateCommand();
                update.CommandText =
                    @"
                        UPDATE dle_vnr_gamefiles
                        SET update_time = $newTime
                        WHERE id = $id
                    ";
                var unixTimeLong = Convert.ToInt64(unixTime);
                update.Parameters.AddWithValue("$newTime", TimestampToDateTime(unixTimeLong));
                update.Parameters.AddWithValue("$id", id);

                update.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// del_vnr_comments 转Subtitles
        /// 注意update_user_id 是0 表示null
        /// user_id 为0的 user_id_old 必不为0
        /// Transaction
        /// </summary>
        public void UpdateVnrComments()
        {
            using var connection = new SqliteConnection("DataSource=C:\\Users\\k1mlka\\Downloads\\ehserver.sqlite");
            connection.Open();

            var query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, timestamp, update_timestamp FROM dle_vnr_comments_copy1
                ";
            using var reader = query.ExecuteReader();


            using var transaction = connection.BeginTransaction();

            const string updateStr = 
                @"
                    UPDATE dle_vnr_comments_copy1 
                    SET time = $Time, update_time = $newTime
                    WHERE id = $id
                ";

            var count = 0;
            while (reader.Read())
            {
                count++;
                if (count % 1000 == 0)
                    Console.WriteLine(count); // 1810000 id=?

                var id = reader.GetString(0);
                var unixTime = reader.GetString(1);
                var unixTimeUpdate = reader.GetString(2);

                var unixTimeLong = Convert.ToInt64(unixTime);
                var unixTimeLongUpdate = Convert.ToInt64(unixTimeUpdate);

                SqliteCommand update = connection.CreateCommand();
                update.CommandText = updateStr;
                update.Transaction = transaction;

                update.Parameters.AddWithValue("$Time", TimestampToDateTime(unixTimeLong));
                update.Parameters.AddWithValue("$newTime", TimestampToDateTime(unixTimeLongUpdate));
                update.Parameters.AddWithValue("$id", id);
                update.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        /// <summary>
        /// CreatorId：如果原id为0 则插入old id
        /// EditorId: 原id为0 插入null
        /// </summary>
        public void Comments2Subtitle()
        {
            SqliteCommand SqliteCommand(SqliteConnection sqliteConnection, string s, SqliteTransaction sqliteTransaction, string id,
                string context, string creationSubtitle, string creationTime, string creatorId, string deleted, string downVote,
                string editorId, string gameId, string hash, string language, string revisionSubtitle, string revisionTime,
                string size, string upVote)
            {
                SqliteCommand update = sqliteConnection.CreateCommand();
                update.CommandText = s;
                update.Transaction = sqliteTransaction;

                update.Parameters.AddWithValue("$Id", id);
                update.Parameters.AddWithValue("$Content", context);
                update.Parameters.AddWithValue("$CreationSubtitle", creationSubtitle);
                update.Parameters.AddWithValue("$CreationTime", creationTime);
                update.Parameters.AddWithValue("$CreatorId", creatorId);
                update.Parameters.AddWithValue("$Deleted", deleted);
                update.Parameters.AddWithValue("$DownVote", downVote);
                update.Parameters.AddWithValue("$EditorId", editorId);
                update.Parameters.AddWithValue("$GameId", gameId);
                update.Parameters.AddWithValue("$Hash", hash);
                update.Parameters.AddWithValue("$Language", language);
                update.Parameters.AddWithValue("$RevisionSubtitle", revisionSubtitle);
                update.Parameters.AddWithValue("$RevisionTime", revisionTime);
                update.Parameters.AddWithValue("$Size", size);
                update.Parameters.AddWithValue("$UpVote", upVote);
                return update;
            }

            using var connection = new SqliteConnection("DataSource=C:\\Users\\k1mlka\\Downloads\\ehserver.sqlite");
            connection.Open();
            var insertStr =
                @"
INSERT INTO Subtitles VALUES ($Id, $Content, $CreationSubtitle, $CreationTime, $CreatorId, $Deleted, $DownVote, $EditorId, $GameId, $Hash, $Language, $RevisionSubtitle, $RevisionTime, $Size, $UpVote)
                ";
            var query = connection.CreateCommand();
            query.CommandText =
                @"
                    SELECT id, context, text, time, user_id, deleted, 0, update_user_id, game_id, context_hash, language, update_comment, update_time, context_size, 0, user_id_old
                    FROM dle_vnr_comments_copy1
                    WHERE type='subtitle'
                ";
            using var reader = query.ExecuteReader();
            using var transaction = connection.BeginTransaction();

            var count = 0;
            //Parallel.ForEach(reader, )
            while (reader.Read())
            {
                var id = reader.GetString(0);
                var context = reader.GetString(1);
                var creationSubtitle = reader.GetString(2);
                var creationTime = reader.GetString(3);
                var creatorId = reader.GetString(4);
                var deleted = reader.GetString(5);
                var downVote = reader.GetString(6);
                var editorId = reader.GetString(7);
                var gameId = reader.GetString(8);
                var hash = reader.GetString(9);
                var language = reader.GetString(10);
                var revisionSubtitle = reader.GetString(11);
                var revisionTime = reader.GetString(12);
                var size = reader.GetString(13);
                var upVote = reader.GetString(14);

                var userIdOld = reader.GetString(15);

                if (Convert.ToInt32(creatorId) == 0)
                {
                    creatorId = userIdOld;
                    if (Convert.ToInt32(userIdOld) == 0)
                    {
                        creatorId = "1";
                        Console.WriteLine("Should never happens. change to ghostId");
                    }
                }

                if (Convert.ToInt32(editorId) == 0)
                {
                    editorId = "1";
                }

                count++;
                if (count % 10000 == 0)
                    Console.WriteLine($"{count} Id={id}");

                var update = SqliteCommand(connection, insertStr, transaction, id, context, creationSubtitle, creationTime, creatorId, deleted, downVote, editorId, gameId, hash, language, revisionSubtitle, revisionTime, size, upVote);
                try
                {
                    update.ExecuteNonQuery();
                }
                catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
                {
                    // :“SQLite Error 19: 'FOREIGN KEY constraint failed'.”
                    // 字幕表中有user_id 184 的人，v2服务器上没这人
                    Console.WriteLine("ghost user");
                    var cmd = SqliteCommand(connection, insertStr, transaction, id, context, creationSubtitle, creationTime, "1", deleted, downVote, editorId, gameId, hash, language, revisionSubtitle, revisionTime, size, upVote);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqliteException exAgain) when (exAgain.SqliteErrorCode == 19)
                    {
                        // check user_id update_user_id game_id etc dependence
                        if (Convert.ToInt32(gameId) == 40655)
                        {
                            // nonexistent game and only with one useless subtitle info
                            continue;
                        }

                        // Editor Id 也设置 1
                        Console.WriteLine("ghost user both creator and editor");
                        var cmd2 = SqliteCommand(connection, insertStr, transaction, id, context, creationSubtitle,
                            creationTime, "1", deleted, downVote, "1", gameId, hash, language,
                            revisionSubtitle, revisionTime, size, upVote);
                        cmd2.ExecuteNonQuery();
                        // 调试模式手动检查原因
                        //throw;
                    }
                }
            }
            transaction.Commit();
        }

        public static DateTime TimestampToDateTime(long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }
    }
}