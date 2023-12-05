﻿using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [Fact]
    public void Any()
    {
        //Arrange
        IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
        IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
        IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

        //Act
        var resultFull = testListFull.Any();
        var resultOne = testListOne.Any();
        var resultNone = testListNone.Any();

        //Assert
        Assert.True(resultFull);
        Assert.True(resultOne);
        Assert.False(resultNone);
    }

    [Fact]
    public void Any_Predicate()
    {
        //Arrange
        var queryable = User.GenerateSampleModels(100).AsQueryable();

        //Act
        bool expected = queryable.Any(u => u.Income > 50);
        bool result = queryable.Any("Income > 50");

        //Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Any_Predicate_WithArgs()
    {
        const int value = 50;

        //Arrange
        var queryable = User.GenerateSampleModels(100).AsQueryable();

        //Act
        bool expected = queryable.Any(u => u.Income > value);
        bool result = queryable.Any("Income > @0", value);

        //Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Any_Dynamic_Select()
    {
        // Arrange
        IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

        // Act
        var expected = queryable.Select(x => x.Roles.Any()).ToArray();
        var result = queryable.Select("Roles.Any()").ToDynamicArray<bool>();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Any_Dynamic_Where()
    {
        const string search = "e";

        // Arrange
        var testList = User.GenerateSampleModels(10);
        var queryable = testList.AsQueryable();

        // Act
        var expected = queryable.Where(u => u.Roles.Any(r => r.Name.Contains(search))).ToArray();
        var result = queryable.Where("Roles.Any(Name.Contains(@0))", search).ToArray();

        Assert.Equal(expected, result);
    }

    // https://dynamiclinq.codeplex.com/discussions/654313
    [Fact]
    public void Any_Dynamic_Where_Nested()
    {
        const string search = "a";

        // Arrange
        var testList = User.GenerateSampleModels(10);
        var queryable = testList.AsQueryable();

        // Act
        var expected = queryable.Where(u => u.Roles.Any(r => r.Permissions.Any(p => p.Name.Contains(search)))).ToArray();
        var result = queryable.Where("Roles.Any(Permissions.Any(Name.Contains(@0)))", search).ToArray();

        Assert.Equal(expected, result);
    }

    // http://stackoverflow.com/questions/30846189/nested-any-in-is-not-working-in-dynamic-linq
    [Fact]
    public void Any_Dynamic_Where_Nested2()
    {
        // arrange
        var list = new List<A>
        {
            new A {Bs = new List<B> {new B {A = new A(), Cs = new List<C> {new C {B = new B()}}}}}
        };
        var queryable = list.AsQueryable();

        // act : 1
        var result1 = queryable.Where("(Name = \"\") && (Bs.Any(Cs.Any()))").ToList();
        var expected1 = queryable.Where(a => a.Name == "" && a.Bs.Any(b => b.Cs.Any()));
        Assert.Equal(expected1, result1);

        // act : 2
        var result2 = queryable.Where("(Bs.Any(Cs.Any())) && (Name = \"\")").ToList();
        var expected2 = queryable.Where(a => a.Bs.Any(b => b.Cs.Any() && a.Name == ""));
        Assert.Equal(expected2, result2);
    }

    // #540
    [Fact]
    public void Any_SubList()
    {
        var organizations = new List<Organization>
        {
            new()
            {
                Code = "OrganizationA",
                Sites = new List<Site>
                {
                    new()
                    {
                        Code = "SiteNorth1"
                    },
                    new()
                    {
                        Code = "SiteNorth2"
                    }
                }
            },
            new()
            {
                Code = "OrganizationB",
                Sites = new List<Site>
                {
                    new()
                    {
                        Code = "SiteNorth3"
                    },
                    new()
                    {
                        Code = "SiteNorth4"
                    }
                }
            },
            new()
            {
                Code = "OrganizationC",
                Sites = new List<Site>
                {
                    new()
                    {
                        Code = "SiteNorth1"
                    },
                    new()
                    {
                        Code = "SiteNorth5"
                    }
                }
            }
        }.AsQueryable();

        var siteFilters = new List<string> { "SiteNorth1", "SiteNorth5" };

        // Working by accessing from organizations with predicate params for an equal expression
        var test2 = organizations.Where("Sites.Any(Code = @0)", siteFilters.First()).ToDynamicArray();
        test2.Should().HaveCount(2);

        // Working by accessing from organizations with classic LINQ
        var test3 = organizations.Where(_ => _.Sites.Any(_ => siteFilters.Contains(_.Code))).ToDynamicArray();
        test3.Should().HaveCount(2);

        // Working by accessing from organizations with string predicate
        var test5 = organizations.Where("Sites.Any(Code in (\"SiteNorth1\", \"SiteNorth5\"))").ToDynamicArray();
        test5.Should().HaveCount(2);

        // Not working : error : System.Linq.Dynamic.Core.Exceptions.ParseException: '')' or ',' expected'
        var test6 = organizations.Where("Sites.Any(Code in @0)", siteFilters).ToDynamicArray();
        test6.Should().HaveCount(2);
    }

    public class Organization
    {
        public string Code { get; set; }

        public ICollection<Site> Sites { get; set; }
    }

    public class Site
    {
        public string Code { get; set; }
    }

    class A
    {
        public string Name { get; set; }
        public IList<B> Bs
        {
            get { return bs; }
            set { bs = value; }
        }
        private IList<B> bs = new List<B>(0);
    }

    class B
    {
        public A A { get; set; }
        public IList<C> Cs
        {
            get { return cs; }
            set { cs = value; }
        }
        private IList<C> cs = new List<C>(0);
    }

    class C
    {
        public B B { get; set; }
    }
}