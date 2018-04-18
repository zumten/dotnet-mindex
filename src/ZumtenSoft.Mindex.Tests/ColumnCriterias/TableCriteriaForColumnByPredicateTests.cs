using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Mappings;
using ZumtenSoft.Mindex.Stubs.MajesticMillion;

namespace ZumtenSoft.Mindex.Tests.ColumnCriterias
{
    [TestClass]
    public class TableCriteriaForColumnByPredicateTests
    {
        [TestMethod]
        public void BuildCondition_WhenFilteringWithPredicate_ShouldApplyThisPredicate()
        {
            var rows = SiteRankingCollections.First10000Rows;

            Func<string, char, bool> containsLetter = (s, c) => s.Contains(c);
            var expected = rows.Where(x => containsLetter(x.TopLevelDomain, 'c') && containsLetter(x.TopLevelDomain, 'o')).ToList();
            TableMappingByPredicate<SiteRanking, SiteRankingSearch, char> mapping = new TableMappingByPredicate<SiteRanking, SiteRankingSearch, char>(s => s.TLDContainsChar, (r, c) => containsLetter(r.TopLevelDomain, c), false);
            var criteria = mapping.ExtractCriteria(new SiteRankingSearch { TLDContainsChar = new [] { 'c', 'o' } });
            ParameterExpression paramExpr = Expression.Parameter(typeof(SiteRanking), "row");
            var lambda = Expression.Lambda<Func<SiteRanking, bool>>(criteria.BuildCondition(paramExpr), paramExpr);
            var actual = rows.Where(lambda.Compile()).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
