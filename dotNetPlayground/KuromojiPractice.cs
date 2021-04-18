using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ja;
using Lucene.Net.Analysis.Ja.Dict;
using Lucene.Net.Analysis.Ja.TokenAttributes;
using Lucene.Net.Analysis.TokenAttributes;
using System;
using System.IO;
using System.Text;

namespace dotNetPlayground
{
    public class KuromojiPractice
    {
        public KuromojiPractice()
        {
            //Tokenizer tokenizer = Tokenizer.builder().build();
            //for (Token token : tokenizer.tokenize("寿司が食べたい。")) {
            //    System.out.println(token.getSurfaceForm() + "\t" + token.getAllFeatures());
            //}
            //Tokenizer tokenizer = new Tokenizer();
            //List<Token> tokens = tokenizer.tokenize("お寿司が食べたい。");
            //for (Token token : tokens)
            //{
            //    System.out.println(token.getSurface() + "\t" + token.getAllFeatures());
            //}
        }

        public void Main()
        {
            const string s = "関西国際空港";
            Console.WriteLine($"対象の文字列:{s}");

            using var reader = new StringReader(s);

            Tokenizer tokenizer = new JapaneseTokenizer(reader, ReadDict(), false, JapaneseTokenizerMode.NORMAL);

            var tokenStreamComponents = new TokenStreamComponents(tokenizer, tokenizer);
            using var tokenStream = tokenStreamComponents.TokenStream;

            // note:処理の実行前にResetを実行する必要がある
            tokenStream.Reset();

            while (tokenStream.IncrementToken())
            {
                Console.WriteLine("---");
                Console.WriteLine(
                    $"ICharTermAttribute=>{tokenStream.GetAttribute<ICharTermAttribute>().ToString()}");

                Console.WriteLine(
                    $"ITermToBytesRefAttribute#BytesRef=>{tokenStream.GetAttribute<ITermToBytesRefAttribute>().BytesRef}");

                Console.WriteLine(
                    $"IOffsetAttribute#StartOffset=>{tokenStream.GetAttribute<IOffsetAttribute>().StartOffset}");
                Console.WriteLine(
                    $"IOffsetAttribute#EndOffset=>{tokenStream.GetAttribute<IOffsetAttribute>().EndOffset}");

                Console.WriteLine(
                    $"IPositionIncrementAttribute=>{tokenStream.GetAttribute<IPositionIncrementAttribute>().PositionIncrement}");
                Console.WriteLine(
                    $"IPositionLengthAttribute=>{tokenStream.GetAttribute<IPositionLengthAttribute>().PositionLength}");

                Console.WriteLine(
                    $"IBaseFormAttribute#GetBaseForm=>{tokenStream.GetAttribute<IBaseFormAttribute>().GetBaseForm()}");

                Console.WriteLine(
                    $"IPartOfSpeechAttribute#GetPartOfSpeech=>{tokenStream.GetAttribute<IPartOfSpeechAttribute>().GetPartOfSpeech()}");

                Console.WriteLine(
                    $"IReadingAttribute#GetReading=>{tokenStream.GetAttribute<IReadingAttribute>().GetReading()}");
                Console.WriteLine(
                    $"IReadingAttribute#GetPronunciation=>{tokenStream.GetAttribute<IReadingAttribute>().GetPronunciation()}");

                Console.WriteLine(
                    $"IInflectionAttribute#GetInflectionForm=>{tokenStream.GetAttribute<IInflectionAttribute>().GetInflectionForm()}");
                Console.WriteLine(
                    $"IInflectionAttribute#GetInflectionType=>{tokenStream.GetAttribute<IInflectionAttribute>().GetInflectionType()}");

                Console.WriteLine("---");
            }
        }

        public static UserDictionary ReadDict()
        {
            TextReader pathReader = new StreamReader(@"", Encoding.UTF8);
            TextReader reader = new StringReader(Dict);
            return new UserDictionary(reader);
        }

        private const string Dict = @"
# Custom segmentation for long entries
日本経済新聞,日本 経済 新聞,ニホン ケイザイ シンブン,カスタム名詞
関西国際空港,関西 国際 空港,カンサイ コクサイ クウコウ,テスト名詞

# Custom reading for sumo wrestler
朝青龍,朝青龍,アサショウリュウ,カスタム人名

# Silly entry:
abcd,a b cd,foo1 foo2 foo3,bar
abcdefg,ab cd efg,foo1 foo2 foo4,bar";
    }
}
