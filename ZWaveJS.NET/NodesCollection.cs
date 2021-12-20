﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace ZWaveJS.NET
{
    public class NodesCollection
    {
        internal NodesCollection()
        {
        }

        internal NodesCollection(ZWaveNode[] Nodes)
        {
            this.Nodes = Nodes.ToList();
        }

        internal List<ZWaveNode> Nodes { get; set; }

        internal void AddNodeToCollection(ZWaveNode Node)
        {
            Nodes.Add(Node);
        }

        internal void ReplaceInformation(ZWaveNode Node)
        {
            ZWaveNode CN = Get(Node.nodeId);

            PropertyInfo[] infos = typeof(ZWaveNode).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                info.SetValue(CN, info.GetValue(Node, null), null);
            }

        }

        internal void RemoveNodeFromCollection(int Node)
        {
            ZWaveNode N = Get(Node);
            if (N != null)
            {
                Nodes.Remove(N);

            }

        }

        public ZWaveNode Get(int Node)
        {
            return Nodes.FirstOrDefault((N) => N.nodeId.Equals(Node));
        }

        public ZWaveNode[] AsArray()
        {
            return Nodes.ToArray();
        }
    }
}
