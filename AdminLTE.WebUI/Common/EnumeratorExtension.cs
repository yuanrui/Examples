using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Common
{
    public static class EnumeratorExtension
    {
        private static void GetChildrenTreeNode<TEntity, TKey, TResult>(IEnumerable<TEntity> sources,
            IList<TreeNode<TResult, TKey>> targets,
            TEntity parentEntity,
            Func<TEntity, TKey> keyFunc, Func<TEntity, TKey> parentKeyFunc,
            Action<TreeNode<TResult, TKey>, TEntity> fixNodeAction, Func<TEntity, TResult> getResultFunc) where TEntity : class
        {
            if (sources == null)
            {
                return;
            }

            //构造查找子节点(子分类)的委托方法
            Func<TEntity, bool> findChildrenFunc = entity =>
            {
                return keyFunc.Invoke(parentEntity).Equals(parentKeyFunc.Invoke(entity));
            };

            var children = sources.Where(findChildrenFunc);
            if (children != null && children.Any())
            {
                //遍历parentEntity下的所有子节点
                foreach (var entity in children)
                {
                    var treeNode = new TreeNode<TResult, TKey>();
                    treeNode.Id = keyFunc(entity);
                    treeNode.ParentId = parentKeyFunc(entity);
                    if (getResultFunc != null)
                    {
                        treeNode.Data = getResultFunc(entity);
                    }
                    treeNode.Children = new List<TreeNode<TResult, TKey>>();

                    if (fixNodeAction != null)
                    {
                        fixNodeAction(treeNode, entity);
                    }
                    targets.Add(treeNode);

                    //递归获取子树
                    GetChildrenTreeNode(sources, treeNode.Children, entity, keyFunc, parentKeyFunc, fixNodeAction, getResultFunc);
                }
            }
        }

        public static IEnumerable<TreeNode<TEntity, TKey>> GetTreeNode<TEntity, TKey>(this IEnumerable<TEntity> sources,
            Func<TEntity, bool> rootFunc, Func<TEntity, TKey> keyFunc, Func<TEntity, TKey> parentKeyFunc,
            Action<TreeNode<TEntity, TKey>, TEntity> fixNodeAction) where TEntity : class
        {
            return GetTreeNode(sources, rootFunc, keyFunc, parentKeyFunc, fixNodeAction, m => m);
        }

        public static IEnumerable<TreeNode<TResult, TKey>> GetTreeNode<TEntity, TKey, TResult>(this IEnumerable<TEntity> sources,
            Func<TEntity, bool> rootFunc, Func<TEntity, TKey> keyFunc, Func<TEntity, TKey> parentKeyFunc,
            Action<TreeNode<TResult, TKey>, TEntity> fixNodeAction, Func<TEntity, TResult> getResultFunc) where TEntity : class
        {
            var result = new List<TreeNode<TResult, TKey>>();
            if (sources == null)
            {
                return result;
            }

            //查找根节点
            var roots = sources.Where(rootFunc);
            if (roots != null && roots.Any())
            {
                //遍历根节点
                foreach (var entity in roots)
                {
                    var treeNode = new TreeNode<TResult, TKey>();
                    treeNode.Id = keyFunc(entity);
                    treeNode.ParentId = parentKeyFunc(entity);
                    if (getResultFunc != null)
                    {
                        treeNode.Data = getResultFunc(entity);
                    }
                    treeNode.Children = new List<TreeNode<TResult, TKey>>();
                    treeNode.Open = false;
                    if (fixNodeAction != null)
                    {
                        fixNodeAction(treeNode, entity);
                    }
                    result.Add(treeNode);

                    //添加子树
                    GetChildrenTreeNode(sources, treeNode.Children, entity, keyFunc, parentKeyFunc, fixNodeAction, getResultFunc);
                }
            }

            return result;
        }

        /// <summary>
        /// 将IEnumerable&lt;T&gt;转化为Pager&lt;TEntity&gt;
        /// 适用于集合已经排序的情况下
        /// Author:YuanRui
        /// </summary>
        /// <typeparam name="TEntity">类型TEntity</typeparam>
        /// <param name="sources">IEnumerable接口</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static Pager<TEntity> AsPager<TEntity>(this IEnumerable<TEntity> sources, int pageIndex, int pageSize) where TEntity : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 10;
            }

            if (sources == null)
            {
                sources = new List<TEntity>();
            }

            var pager = new Pager<TEntity>(pageSize, pageIndex, sources.Count(), sources.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList());

            return pager;
        }

        /// <summary>
        /// 将IEnumerable&lt;T&gt;转化为Pager&lt;TEntity&gt;
        /// Author:YuanRui
        /// </summary>
        /// <typeparam name="TEntity">类型TEntity</typeparam>
        /// <typeparam name="TKey">类型TEntity中的成员属性的类型</typeparam>
        /// <param name="sources">IEnumerable接口</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderExp">排序的lambda表达式 如:m=>m.id</param>
        /// <param name="ascending">是否为升序 默认为升序</param>
        /// <returns></returns>
        public static Pager<TEntity> AsPager<TEntity, TKey>(this IEnumerable<TEntity> sources, int pageIndex, int pageSize,
            Func<TEntity, TKey> orderExp, bool ascending) where TEntity : class
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            if (sources == null)
            {
                sources = new List<TEntity>();
            }
            List<TEntity> data = null;

            if (ascending)
            {
                data = sources.OrderBy(orderExp).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }
            else
            {
                data = sources.OrderByDescending(orderExp).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            }

            var pager = new Pager<TEntity>(pageSize, pageIndex, sources.Count(), data);

            return pager;
        }
    }
}