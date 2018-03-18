using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Indexes;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests.Columns
{
    [TestClass]
    public class TableColumnTests
    {
        [TestMethod]
        public void GetScore_SearchEmptyCriteria_ShouldReturnScoreOf_1()
        {
            var rows = SiteRankingCollections.FirstTenRows;
            var search = new SiteRankingSearch();
            var expectedScore = new TableColumnScore(1, false);

            TestGetScores(rows, search, expectedScore);
        }

        [TestMethod]
        public void GetScore_SearchByValues_XOutOf5_ShouldReturnScoreOf_X_Times_0_2()
        {
            SiteRanking[] rows =
            {
                new SiteRanking { TopLevelDomain = "com" },
                new SiteRanking { TopLevelDomain = "com" },
                new SiteRanking { TopLevelDomain = "com" },
                new SiteRanking { TopLevelDomain = "org" },
                new SiteRanking { TopLevelDomain = "org" },
                new SiteRanking { TopLevelDomain = "net" },
                new SiteRanking { TopLevelDomain = "ca" },
                new SiteRanking { TopLevelDomain = "ca" },
                new SiteRanking { TopLevelDomain = "uk" },
                new SiteRanking { TopLevelDomain = "uk" },
            };

            TestGetScores(rows,
                new SiteRankingSearch { TopLevelDomain = new[] { "net" } },
                new TableColumnScore(0.2f, true));

            TestGetScores(rows,
                new SiteRankingSearch { TopLevelDomain = new[] { "com", "org" } },
                new TableColumnScore(0.4f, true));
        }


        [TestMethod]
        public void GetScore_SearchByPredicate_ShouldReturnScoreOf_1()
        {
            var rows = SiteRankingCollections.FirstTenRows;
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByPredicate((string x) => x.StartsWith("c")) };
            var expectedScore = new TableColumnScore(1, false);

            TestGetScores(rows, search, expectedScore);
        }

        [TestMethod]
        public void GetScore_SearchByRange_ShouldReturnScoreOf_Ratio()
        {
            var rows = SiteRankingCollections.FirstTenRows;
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByRange("ca", "com") };
            var expectedScore = new TableColumnScore(0.5f, false);

            TestGetScores(rows, search, expectedScore);
        }

        private static void TestGetScores(SiteRanking[] rows, SiteRankingSearch search, TableColumnScore expectedScore)
        {
            TableColumn<SiteRanking, SiteRankingSearch, string> column = new TableColumn<SiteRanking, SiteRankingSearch, string>(rows, x => x.TopLevelDomain, x => x.TopLevelDomain, StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            var actualScore = column.ExtractCriteria(search)?.Score ?? TableColumnScore.NotOptimizable;

            Assert.AreEqual(expectedScore, actualScore);
        }
    }
}
