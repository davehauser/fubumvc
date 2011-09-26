using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using FubuCore.Reflection;
using FubuLocalization;
using FubuMVC.Core.Rest.Media.Projections;

namespace FubuMVC.Core.Rest.Media.Atom
{
    // Just use SyndicationLink

    public class Feed<T>
    {
        private readonly IList<Action<SyndicationFeed>> _alterations = new List<Action<SyndicationFeed>>();
        private readonly IList<FeedItem<T>> _maps = new List<FeedItem<T>>();

        public SyndicationFeed CreateFeed(DateTime updated, IEnumerable<IValues<T>> targets)
        {
            var feed = new SyndicationFeed{
                LastUpdatedTime = updated,
                Items = targets.Select(target =>
                {
                    var item = new SyndicationItem();
                    _maps.Each(map => map.ConfigureItem(item, target));

                    return item;
                })
            };

            _alterations.Each(x => x(feed));

            return feed;
        }

        public void Title(StringToken title)
        {
            throw new NotImplementedException();
        }

        public void Description(StringToken description)
        {
            throw new NotImplementedException();
        }

        public void Items<TMap>() where TMap : FeedItem<T>
        {
            throw new NotImplementedException();
        }

        public void Items(Action<FeedItem<T>> configure)
        {
            throw new NotImplementedException();
        }
    }

    public static class SyndicationStringExtensions
    {
        public static TextSyndicationContent ToContent(this string text)
        {
            return new TextSyndicationContent(text);
        }
    }

    public class FeedItem<T>
    {
        private readonly IList<Action<IValues<T>, SyndicationItem>> _modifications
            = new List<Action<IValues<T>, SyndicationItem>>();

        public FeedItem()
        {
        }

        public FeedItem(Action<FeedItem<T>> configure)
        {
            configure(this);
        }

        private Action<IValues<T>, SyndicationItem> alter
        {
            set { _modifications.Add(value); }
        }

        public void ConfigureItem(SyndicationItem item, IValues<T> target)
        {
            _modifications.Each(x => x(target, item));
        }

        private void modify<TArg>(Expression<Func<T, TArg>> property, Action<SyndicationItem, TArg> modification)
        {
            var accessor = ReflectionHelper.GetAccessor(property);
            alter = (target, item) =>
            {
                var value = target.ValueFor(accessor);

                if (value != null)
                {
                    modification(item, (TArg) value);
                }
            };
        }


        public void Title(Expression<Func<T, string>> expression)
        {
            modify(expression, (item, value) => item.Title = value.ToContent());
        }

        public void Id(Expression<Func<T, object>> expression)
        {
            modify(expression, (item, value) => item.Id = value.ToString());
        }

        public void UpdatedByProperty(Expression<Func<T, DateTime>> property)
        {
            var accessor = ReflectionHelper.GetAccessor(property);
            alter = (target, item) => item.LastUpdatedTime = (DateTime) target.ValueFor(accessor);
        }


        public void Extension(string xsd, Action<Projection<T>> configure)
        {
            throw new NotImplementedException();
        }

        public void Extension<TProjection>(string xsd) where TProjection : Projection<T>
        {
            throw new NotImplementedException();
        }
    }
}