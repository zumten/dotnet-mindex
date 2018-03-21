using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests.ColumnCriterias
{
    [TestClass]
    public class TableCriteriaForColumnByValueTests
    {
        [TestMethod]
        public void GetScore_CriteriaEmpty_ShouldReturnScoreOf_1()
        {
            var rows = SiteRankingCollections.FirstTenRows;
            var search = new SiteRankingSearch();
            var expectedScore = new TableCriteriaScore(1, false);

            TestGetScores(rows, search, expectedScore);
        }

        [TestMethod]
        public void GetScore_CriteriaByValues_XOutOf5_ShouldReturnScoreOf_X_Times_0_2()
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
                new TableCriteriaScore(0.2f, true));

            TestGetScores(rows,
                new SiteRankingSearch { TopLevelDomain = new[] { "com", "org" } },
                new TableCriteriaScore(0.4f, true));
        }


        [TestMethod]
        public void GetScore_CriteriaByPredicate_ShouldReturnScoreOf_1()
        {
            var rows = SiteRankingCollections.First10000Rows;
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByPredicate((string x) => x.StartsWith("c")) };
            var expectedScore = new TableCriteriaScore(1, false);

            TestGetScores(rows, search, expectedScore);
        }

        [TestMethod]
        public void GetScore_CriteriaByRange_ShouldReturnScoreOf_Ratio()
        {
            var rows = SiteRankingCollections.FirstTenRows;
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByRange("ca", "com") };
            var expectedScore = new TableCriteriaScore(0.5f, true);

            TestGetScores(rows, search, expectedScore);
        }

        [TestMethod]
        public void Reduce_CriteriaByValues_ShouldReturnSubset()
        {
            var rows = SiteRankingCollections.First10000Rows.OrderBy(x => x.TopLevelDomain).ToArray();
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByValues("ca", "com") };
            var criteria = BuildCriteria(rows, search);
            var expected = rows.Where(x => x.TopLevelDomain == "ca" || x.TopLevelDomain == "com");
            var actual = criteria.Reduce(new BinarySearchResult<SiteRanking>(rows));
            CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void Reduce_CriteriaByRange_ShouldReturnSubset()
        {
            var rows = SiteRankingCollections.First10000Rows.OrderBy(x => x.TopLevelDomain).ToArray();
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByRange("com", "net") };
            var criteria = BuildCriteria(rows, search);
            var expected = rows.Where(x => StringComparer.OrdinalIgnoreCase.Compare(x.TopLevelDomain, "com") >=0 && StringComparer.OrdinalIgnoreCase.Compare(x.TopLevelDomain, "net") <= 0);
            var actual = criteria.Reduce(new BinarySearchResult<SiteRanking>(rows));
            CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void Reduce_CriteriaByPredicate_ShouldReturnNull()
        {
            var rows = SiteRankingCollections.First10000Rows;
            var search = new SiteRankingSearch { TopLevelDomain = SearchCriteria.ByPredicate((string x) => x == "ca") };
            var criteria = BuildCriteria(rows, search);
            var actual = criteria.Reduce(new BinarySearchResult<SiteRanking>(rows));
            Assert.IsNull(actual);
        }

        private static ITableCriteriaForColumn<SiteRanking, SiteRankingSearch> BuildCriteria(SiteRanking[] rows, SiteRankingSearch search)
        {
            TableColumnByValue<SiteRanking, SiteRankingSearch, string> column = new TableColumnByValue<SiteRanking, SiteRankingSearch, string>(rows, x => x.TopLevelDomain, x => x.TopLevelDomain, StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            return column.ExtractCriteria(search);
        }

        private static void TestGetScores(SiteRanking[] rows, SiteRankingSearch search, TableCriteriaScore expectedScore)
        {
            var actualScore = BuildCriteria(rows, search)?.Score ?? TableCriteriaScore.NotOptimizable;
            Assert.AreEqual(expectedScore, actualScore);
        }
    }
}
