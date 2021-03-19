using Microsoft.Data.Sqlite;

namespace MicrosoftSqlitePractice
{
    public class EhServerGameUserId0To1
    {
        /// <summary>
        /// ~~userId 0 is guest, but guest id is 1 in new Users table~~
        /// Users 0 is ghost
        /// Users 4 is guest
        /// 原本的 user 有0，不应有0，全部改为1幽灵。没有问题
        /// </summary>
        public EhServerGameUserId0To1()
        {
            using var connection = new SqliteConnection("DataSource=C:\\Users\\k1mlka\\Downloads\\ehserver.sqlite");
            connection.Open();

            var query = connection.CreateCommand();
            query.CommandText =
                @"
                    Update dle_vnr_gamefiles
                    SET update_user_id=1
                    WHERE update_user_id=0
                ";
            query.ExecuteNonQuery();
        }
    }
}