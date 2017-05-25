using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Common
{
    public class TreeNode
    {
        private const string _open = "open";
        private const string _closed = "closed";

        private TreeNodeModel _model;

        public TreeNode()
        {
            _model = new TreeNodeModel();
            _model.state = _open;

            _model.children = new Collection<TreeNodeModel>();

            Checked = false;
        }

        public string Id
        {
            get { return _model.id; }
            set { _model.id = value; }
        }

        public string Text
        {
            get { return _model.text; }
            set { _model.text = value; }
        }

        public bool Checked
        {
            get { return _model.@checked; }
            set { _model.@checked = value; }
        }

        public bool Open
        {
            get { return _model.state == _open; }
            set { _model.state = value ? _open : _closed; }
        }

        public int Order { get; set; }

        public string IconClass
        {
            get { return _model.iconCls; }
            set { _model.iconCls = value; }
        }

        public string Url { get; set; }

        public ICollection<TreeNode> Children { get; set; }

        public TreeNodeModel AsModel()
        {
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    _model.children.Add(child.AsModel());
                }
            }

            return _model;
        }

        public string ActiveClass
        {
            get
            {
                return Checked ? "active" : string.Empty;
            }
        }

        public string GetIconHtml(string defaultIcon = null)
        {
            if (string.IsNullOrEmpty(defaultIcon))
            {
                defaultIcon = "fa fa-circle-o";
            }

            return string.IsNullOrEmpty(IconClass) ? "<i class='" + defaultIcon + "'></i>" : "<i class='" + IconClass + "'></i>";
        }
    }

    public class TreeNodeModel
    {
        public string id;

        public string text;

        public string iconCls;

        public string state;

        public bool @checked;

        public ICollection<TreeNodeModel> children;

    }


    public static class TreeNodeExtention
    {
        public static IEnumerable<TreeNodeModel> AsModels(this IEnumerable<TreeNode> nodes)
        {

            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            var result = new List<TreeNodeModel>();
            foreach (var node in nodes)
            {
                result.Add(node.AsModel());
            }

            return result;
        }
    }

    public class TreeNode<TEntity, TKey> //: ITreeNode<TEntity, TKey>
    {
        private const string _open = "open";
        private const string _closed = "closed";

        private TreeNodeModel<TEntity, TKey> _model;

        public TKey Id
        {
            get { return _model.id; }
            set { _model.id = value; }
        }

        public TKey ParentId
        {
            get { return _model._parentId; }
            set { _model._parentId = value; }
        }

        public TEntity Data
        {
            get { return _model.attributes; }
            set { _model.attributes = value; }
        }

        public string Text
        {
            get { return _model.text; }
            set { _model.text = value; }
        }

        public bool? Checked
        {
            get { return _model.@checked; }
            set { _model.@checked = value; }
        }

        public bool Open
        {
            get { return _model.state == _open; }
            set { _model.state = value ? _open : _closed; }
        }

        public string IconClass
        {
            get { return _model.iconCls; }
            set { _model.iconCls = value; }
        }

        public IList<TreeNode<TEntity, TKey>> Children { get; set; }

        public TreeNode()
        {
            _model = new TreeNodeModel<TEntity, TKey>();
            _model.state = _open;

            _model.children = new Collection<TreeNodeModel<TEntity, TKey>>();

            Checked = null;
        }

        public TreeNodeModel<TEntity, TKey> AsModel()
        {
            if (Children != null && Children.Any())
            {
                foreach (var child in Children)
                {
                    _model.children.Add(child.AsModel());

                    // 针对EasyUI的Tree进行优化处理 子节点全部选中父节点才可选中
                    if (_model.children.All(m => m.@checked.HasValue && m.@checked.Value))
                    {
                        _model.@checked = true;
                    }
                    else
                    {
                        _model.@checked = null;
                    }
                }
            }
            else
            {
                Open = true;
            }

            return _model;
        }

        public TreeNodeModel<TResult, TKey> AsModel<TResult>(Func<TreeNode<TEntity, TKey>, TResult> selectorFunc)
        {
            var result = new TreeNodeModel<TResult, TKey>();
            result.id = _model.id;
            result._parentId = _model._parentId;
            result.text = _model.text;
            result.iconCls = _model.iconCls;
            result.state = _model.state;
            result.@checked = _model.@checked;
            result.attributes = selectorFunc(this);
            result.children = new Collection<TreeNodeModel<TResult, TKey>>();

            if (Children != null && Children.Any())
            {
                foreach (var child in Children)
                {
                    result.children.Add(child.AsModel(selectorFunc));

                    // 针对EasyUI的Tree进行优化处理 子节点全部选中父节点才可选中
                    if (result.children.All(m => m.@checked.HasValue && m.@checked.Value))
                    {
                        result.@checked = true;
                    }
                    else
                    {
                        result.@checked = null;
                    }
                }
            }
            else
            {
                Open = true;
            }

            return result;
        }
    }

    public struct TreeNodeModel<TEntity, TKey>
    {
        public TKey id;

        public TKey _parentId;

        public string text;

        public string iconCls;

        public string state;

        public bool? @checked;

        public TEntity attributes;

        public ICollection<TreeNodeModel<TEntity, TKey>> children;
    }

    public static class TreeNodeExtention2
    {
        public static IEnumerable<TreeNodeModel<TEntity, TKey>> AsModels<TEntity, TKey>(this IEnumerable<TreeNode<TEntity, TKey>> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            var result = new List<TreeNodeModel<TEntity, TKey>>();
            foreach (var node in nodes)
            {
                result.Add(node.AsModel());
            }

            return result;
        }

        public static IEnumerable<TreeNodeModel<TResult, TKey>> AsModels<TEntity, TKey, TResult>(this IEnumerable<TreeNode<TEntity, TKey>> nodes, Func<TreeNode<TEntity, TKey>, TResult> selectorFunc)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            var result = new List<TreeNodeModel<TResult, TKey>>();
            foreach (var node in nodes)
            {
                result.Add(node.AsModel(selectorFunc));
            }

            return result;
        }
    }
}