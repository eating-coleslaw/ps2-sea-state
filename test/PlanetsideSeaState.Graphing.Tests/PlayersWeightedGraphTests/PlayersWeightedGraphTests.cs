using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlanetsideSeaState.Graphing.Tests.PlayersWeightedGraphTests
{
    public class PlayersWeightedGraphTests
    {
        private static readonly DateTime _olderTime = new(2021, 4, 1, 12, 0, 0);
        private static readonly DateTime _seedTime = new(2021, 5, 1, 12, 0, 0);
        private static readonly DateTime _newerTime = new(2021, 6, 1, 12, 0, 0);

        public class EmptyGraph
        {
            private readonly PlayersWeightedGraph _graph;

            public EmptyGraph()
            {
                _graph = new();
            }

            #region Initial State
            [Fact]
            public void PlayerConnections_IsEmpty()
            {
                Assert.Empty(_graph.ReadOnlyPlayerConnections);
            }

            [Fact]
            public void GetPlayerNode_ReturnsNull()
            {
                Assert.Null(_graph.GetPlayerNode("1"));
            }

            [Fact]
            public void GetPlayerConnectionsSnapshot_ReturnsNull()
            {
                Assert.Null(_graph.GetPlayerConnectionsSnapshot("1"));
            }
            
            [Fact]
            public void GetPlayerNeighborsSnapshot_ReturnsNull()
            {
                Assert.Null(_graph.GetPlayerNeighborsSnapshot("1"));
            }
            #endregion Initial State
        }

        public class SingleNodeGraph
        {
            private readonly PlayersWeightedGraph _graph;
            private readonly PlayerNode _nodeA;

            public SingleNodeGraph()
            {
                _graph = new();
                _nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);
            }

            [Fact]
            public async Task AddNodeAsync_PlayerConnections_ContainsNodeA()
            {
                await _graph.AddNodeAsync(_nodeA);

                Assert.Contains(_nodeA, _graph.ReadOnlyPlayerConnections);
            }
            
            [Fact]
            public async Task AddNodeAsync_PlayerConnections_NodeAValueIsNotNull()
            {
                await _graph.AddNodeAsync(_nodeA);

                _graph.ReadOnlyPlayerConnections.TryGetValue(_nodeA, out HashSet<PlayerEdge> value);

                Assert.NotNull(value);
            }

            [Fact]
            public async Task GetPlayerNode_ReturnsNodeA()
            {
                await _graph.AddNodeAsync(_nodeA);

                var result = _graph.GetPlayerNode("1");

                Assert.Equal(_nodeA, result);
            }
        }

        #region params Constructor
        


        #endregion params Constructor

        #region Add Node
        [Fact]
        public void AddNode_NewNode_ReturnsTrue()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            var result = graph.AddNode(nodeA);

            Assert.False(result);
        }

        [Fact]
        public void AddNode_NewNode_CountIs1()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            graph.AddNode(nodeA);

            Assert.NotEqual(1, graph.PlayerCount);
        }

        [Fact]
        public void AddNode_NewNode_GraphContainsNode()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            graph.AddNode(nodeA);

            Assert.Contains(nodeA, graph.ReadOnlyPlayerConnections);
        }

        [Fact]
        public void AddNode_DuplicateNode_ReturnsFalse()
        {
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);
            var graph = new PlayersWeightedGraph(nodeA);

            var result = graph.AddNode(nodeA);

            Assert.True(result);
        }

        [Fact]
        public void AddNode_DuplicateNode_PlayerCountIs1()
        {
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);
            var graph = new PlayersWeightedGraph(nodeA);

            graph.AddNode(nodeA);

            Assert.Equal(1, graph.PlayerCount);
        }

        [Fact]
        public void AddNode_DuplicateNode_GraphContainsNode()
        {
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);
            var graph = new PlayersWeightedGraph(nodeA);

            graph.AddNode(nodeA);

            Assert.Contains(nodeA, graph.ReadOnlyPlayerConnections);
        }
        #endregion Add Node

        #region Add Node Async
        [Fact]
        public async Task AddNodeAsync_NewNode_ReturnsTrue()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            var result = await graph.AddNodeAsync(nodeA);

            Assert.True(result);
        }

        [Fact]
        public async Task AddNodeAsync_NewNode_CountIs1()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            await graph.AddNodeAsync(nodeA);

            Assert.Equal(1, graph.PlayerCount);
        }

        [Fact]
        public async Task AddNodeAsync_DuplicateNode_OnlyOneReturnsTrue()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            var firstTask = graph.AddNodeAsync(nodeA);
            var secondTask = graph.AddNodeAsync(nodeA);

            var taskList = new List<Task<bool>>()
            {
                firstTask,
                secondTask
            };

            var trues = 0;
            var falses = 0;

            while (taskList.Any())
            {
                var finishedTask = await Task.WhenAny(taskList);

                if (finishedTask.Result)
                {
                    trues++;
                }
                else
                {
                    falses++;
                }

                taskList.Remove(finishedTask);
            }

            Assert.Equal(1, trues);
            Assert.Equal(1, falses);
        }

        [Fact]
        public async Task AddNodeAsync_DuplicateNode_PlayerCountIs1()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            var firstTask = graph.AddNodeAsync(nodeA);
            var secondTask = graph.AddNodeAsync(nodeA);

            var taskList = new List<Task<bool>>()
            {
                firstTask,
                secondTask
            };

            var trues = 0;
            var falses = 0;

            while (taskList.Any())
            {
                var finishedTask = await Task.WhenAny(taskList);

                if (finishedTask.Result)
                {
                    trues++;
                }
                else
                {
                    falses++;
                }

                taskList.Remove(finishedTask);
            }

            Assert.Equal(1, graph.PlayerCount);
        }

        [Fact]
        public async Task AddNodeAsync_DuplicateNode_GraphContainsNode()
        {
            var graph = new PlayersWeightedGraph();
            var nodeA = PlayersWeightedGraphHelper.GetA(_seedTime);

            var firstTask = graph.AddNodeAsync(nodeA);
            var secondTask = graph.AddNodeAsync(nodeA);

            var taskList = new List<Task<bool>>()
            {
                firstTask,
                secondTask
            };

            var trues = 0;
            var falses = 0;

            while (taskList.Any())
            {
                var finishedTask = await Task.WhenAny(taskList);

                if (finishedTask.Result)
                {
                    trues++;
                }
                else
                {
                    falses++;
                }

                taskList.Remove(finishedTask);
            }

            Assert.Contains(nodeA, graph.ReadOnlyPlayerConnections);
        }
        #endregion Add Node Async

        #region AddOrUpdateRelation
        
        #endregion AddOrUpdateRelation
    }
}
