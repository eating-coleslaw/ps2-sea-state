using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static PlanetsideSeaState.Graphing.Tests.FacilityGraphHelper;

namespace PlanetsideSeaState.Graphing.Tests
{
    public class FacilityGraphTests
    {
        // Public Methods:
        //  - void AddNode(FacilityNode facilityNode)
        //  - FacilityNode GetNode(int facilityId)
        //  - void AddConnection(int lhsId, int rhsId)
        //  - IEnumerable<FacilityNode> GetNeighbors(FacilityNode facilityNode)
        //  - void MarkMapComplete()
        //  - void MarkMapIncomplete()
        //  - async Task<bool> IsFacilityCapturable(int facilityId)
        //  - async Task UpdateFacilityOwner(FacilityOwnerUpdate update)

        private static readonly DateTime _olderTime = new(2021, 4, 1, 12, 0, 0);
        private static readonly DateTime _seedTime = new(2021, 5, 1, 12, 0, 0);
        private static readonly DateTime _newerTime = new(2021, 6, 1, 12, 0, 0);

        public class EmptyFacilityGraph : IDisposable
        {
            private readonly FacilityGraph _graph;

            public EmptyFacilityGraph()
            {
                _graph = new();
            }

            public void Dispose()
            {
                _graph.Dispose();
            }

            #region Initial State
            [Fact]
            public void InitialState_ZeroEdgeCount()
            {
                Assert.Equal(0, _graph.EdgeCount);
            }
            
            [Fact]
            public void InitialState_ZeroFacilityCount()
            {
                Assert.Equal(0, _graph.FacilityCount);
            }
            
            [Fact]
            public void InitialState_MapIsNotComplete()
            {
                Assert.False(_graph.MapIsComplete);
            }
            #endregion Initial State

            [Fact]
            public void GetNode_NullReturn()
            {
                var node = _graph.GetNode(1);

                Assert.Null(node);
            }

            //[Fact]
            //public void GetNeighbors_NullReturn()
            //{
            //    var neighbors = _graph.GetNeighbors(_graph.GetNode(1));

            //    Assert.Null(neighbors);
            //}

            [Fact]
            public async Task IsFacilityCapturable_FacilityA_KeyNotFoundExceptionThrown()
            {
                var exception = await Record.ExceptionAsync(async () => await _graph.IsFacilityCapturable(1));

                Assert.NotNull(exception);
                Assert.IsType<KeyNotFoundException>(exception);
            }
        }


        public class SingleFacilityGraph : IDisposable
        {
            private readonly FacilityGraph _graph;

            public SingleFacilityGraph()
            {
                _graph = new();

                _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
            }

            public void Dispose()
            {
                _graph.Dispose();
            }


            #region AddNode
            [Fact]
            public void AddNode_AddFacilityA_ZeroEdgeCount()
            {
                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void AddNode_AddFacilityA_OneFacilityCount()
            {
                Assert.Equal(1, _graph.FacilityCount);
            }

            [Fact]
            public void AddNode_AddFacilityA_MapIsNotComplete()
            {
                Assert.False(_graph.MapIsComplete);
            }

            [Fact]
            public void AddNode_AddDuplicateA_DuplicateNodeExceptionThrown()
            {
                Action action = () =>
                {
                    _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                };

                var exception = Record.Exception(action);

                Assert.NotNull(exception);
                Assert.IsType<DuplicateGraphNodeException>(exception);
            }

            [Fact]
            public void AddNode_AddDuplicateA_ZeroEdgeCount()
            {
                Action action = () =>
                {
                    _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                };

                var exception = Record.Exception(action);

                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void AddNode_AddDuplicateA_OneFacilityCount()
            {
                Action action = () =>
                {
                    _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                };

                var exception = Record.Exception(action);

                Assert.Equal(1, _graph.FacilityCount);
            }

            [Fact]
            public void AddNode_AddDuplicateA_MapIsNotComplete()
            {
                Action action = () =>
                {
                    _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                };

                var exception = Record.Exception(action);

                Assert.False(_graph.MapIsComplete);
            }
            #endregion AddNode


            [Fact]
            public void GetNode_GetFacilityA()
            {
                var node = _graph.GetNode(1);

                Assert.Equal(1, node.Id);
            }

            #region GetNeighbors
            [Fact]
            public void GetNeighbors_FacilityA_IsEmpty()
            {
                var neighbors = _graph.GetNeighbors(_graph.GetNode(1));

                Assert.Empty(neighbors);
            }

            [Fact]
            public void GetNeighbors_FacilityA_ZeroEdgeCount()
            {
                var exception = Record.Exception(() => _graph.GetNeighbors(_graph.GetNode(1)));

                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void GetNeighbors_FacilityA_OneFacilityCount()
            {
                var exception = Record.Exception(() => _graph.GetNeighbors(_graph.GetNode(1)));

                Assert.Equal(1, _graph.FacilityCount);
            }

            [Fact]
            public void GetNeighbors_FacilityA_MapIsNotComplete()
            {
                var exception = Record.Exception(() => _graph.GetNeighbors(_graph.GetNode(1)));

                Assert.False(_graph.MapIsComplete);
            }
            #endregion GetNeighbors

            #region AddConnection
            [Fact]
            public void AddConnection_FacilityAToA_ArgumentExceptionThrown()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 1));

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
            }

            [Fact]
            public void AddConnection_FacilityAToA_ZeroEdgeCount()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 1));

                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void AddConnection_FacilityAToA_OneFacilityCount()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 1));

                Assert.Equal(1, _graph.FacilityCount);
            }

            [Fact]
            public void AddConnection_FacilityAToA_MapIsNotComplete()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 1));

                Assert.False(_graph.MapIsComplete);
            }

            [Fact]
            public void AddConnection_FacilityAToNull_KeyNotFoundExceptionThrown()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 2));

                Assert.NotNull(exception);
                Assert.IsType<KeyNotFoundException>(exception);
            }

            [Fact]
            public void AddConnection_FacilityAToNull_ZeroEdgeCount()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 2));

                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void AddConnection_FacilityAToNull_OneFacilityCount()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 2));

                Assert.Equal(1, _graph.FacilityCount);
            }

            [Fact]
            public void AddConnection_FacilityAToNull_MapIsNotComplete()
            {
                var exception = Record.Exception(() => _graph.AddConnection(1, 2));

                Assert.False(_graph.MapIsComplete);
            }
            #endregion AddConnection

            #region UpdateFacilityOwner
            [Fact]
            public async Task UpdateFacilityOwner_NewerTimestamp_TimestampNotChanged()
            {
                FacilityOwnerUpdate update = new(1, 2, _newerTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(_seedTime, nodeA.LastOwnershipTimestamp);
            }

            [Fact]
            public async Task UpdateFacilityOwner_NewerTimestamp_OwnerNotChanged()
            {
                FacilityOwnerUpdate update = new(1, 2, _newerTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(1, nodeA.OwningFactionId);
            }
            #endregion UpdateFacilityOwner

        }

        public class SingleFacilityMapCompleteGraph : IDisposable
        {
            private readonly FacilityGraph _graph;

            public SingleFacilityMapCompleteGraph()
            {
                _graph = new();

                _graph.AddNode(GetFacilityNodeA(_seedTime, 1));

                _graph.MarkMapComplete();
            }

            public void Dispose()
            {
                _graph.Dispose();
            }

            #region UpdateFacilityOwner
            [Fact]
            public async Task UpdateFacilityOwner_OlderTimestamp_KeepTimestamp()
            {
                FacilityOwnerUpdate update = new(1, 2, _olderTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(_seedTime, nodeA.LastOwnershipTimestamp);
            }

            [Fact]
            public async Task UpdateFacilityOwner_OlderTimestamp_KeepOwner()
            {
                FacilityOwnerUpdate update = new(1, 2, _olderTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(1, nodeA.OwningFactionId);
            }

            [Fact]
            public async Task UpdateFacilityOwner_SameTimestamp_UpdateTimestamp()
            {
                FacilityOwnerUpdate update = new(1, 2, _seedTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(_seedTime, nodeA.LastOwnershipTimestamp);
            }


            [Fact]
            public async Task UpdateFacilityOwner_SameTimestamp_UpdateOwner()
            {
                FacilityOwnerUpdate update = new(1, 2, _seedTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(2, nodeA.OwningFactionId);
            }

            [Fact]
            public async Task UpdateFacilityOwner_NewerTimestamp_UpdateTimestamp()
            {
                FacilityOwnerUpdate update = new(1, 2, _newerTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(_newerTime, nodeA.LastOwnershipTimestamp);
            }

            [Fact]
            public async Task UpdateFacilityOwner_NewerTimestamp_UpdateOwner()
            {
                FacilityOwnerUpdate update = new(1, 2, _newerTime);

                await _graph.UpdateFacilityOwner(update);

                var nodeA = _graph.GetNode(1);

                Assert.Equal(2, nodeA.OwningFactionId);
            }
            #endregion UpdateFacilityOwner

        }

        public class ThreeFacilityGraph : IDisposable
        {
            private readonly FacilityGraph _graph;

            public ThreeFacilityGraph()
            {
                _graph = new();

                _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                _graph.AddNode(GetFacilityNodeB(_seedTime, 2));
                _graph.AddNode(GetFacilityNodeC(_seedTime, 2));
            }

            public void Dispose()
            {
                _graph.Dispose();
            }

            #region Initial State
            [Fact]
            public void InitialState_ZeroEdgeCount()
            {
                Assert.Equal(0, _graph.EdgeCount);
            }

            [Fact]
            public void InitialState_ThreeFacilityCount()
            {
                Assert.Equal(3, _graph.FacilityCount);
            }

            [Fact]
            public void InitialState_MapIsComplete_IsFalse()
            {
                Assert.False(_graph.MapIsComplete);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void InitialState_GetNeighbors_IsEmpty(int facilityId)
            {
                var neighbors = _graph.GetNeighbors(_graph.GetNode(facilityId));

                Assert.Empty(neighbors);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public async Task InitialState_IsCapturable_IsFalse(int facilityId)
            {
                var isCapturable = await _graph.IsFacilityCapturable(facilityId);

                Assert.False(isCapturable);
            }

            #endregion Initial State

            #region AddConnection
            [Fact]
            public void AddConnection_AToB_OneEdgeCount()
            {
                _graph.AddConnection(1, 2);

                Assert.Equal(1, _graph.EdgeCount);
            }

            [Fact]
            public void AddConnection_AToB_ThreeFacilityCount()
            {
                _graph.AddConnection(1, 2);

                Assert.Equal(3, _graph.FacilityCount);
            }

            [Fact]
            public void AddConnection_AToB_MapIsNotComplete()
            {
                _graph.AddConnection(1, 2);

                Assert.False(_graph.MapIsComplete);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(3)]
            public async Task AddConnection_AToB_IsCapturableA_IsFalse(int facilityId)
            {
                _graph.AddConnection(1, 2);

                var isCapturable = await _graph.IsFacilityCapturable(facilityId);

                Assert.False(isCapturable);
            }

            [Fact]
            public async Task AddConnection_AToB_IsCapturableB_IsTrue()
            {
                _graph.AddConnection(1, 2);

                var isCapturable = await _graph.IsFacilityCapturable(2);

                Assert.True(isCapturable);
            }
            #endregion AddConnection
        }

        public class ThreeFacilityMapCompleteGraph : IDisposable
        {
            private readonly FacilityGraph _graph;

            public ThreeFacilityMapCompleteGraph()
            {
                _graph = new();

                _graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                _graph.AddNode(GetFacilityNodeB(_seedTime, 2));
                _graph.AddNode(GetFacilityNodeC(_seedTime, 2));

                _graph.MarkMapComplete();
            }

            public void Dispose()
            {
                _graph.Dispose();
            }

            #region Initial State
            [Fact]
            public void InitialState_MapIsComplete_IsTrue()
            {
                Assert.True(_graph.MapIsComplete);
            }

            #endregion Initial State

            #region AddConnection
            [Fact]
            public void AddConnection_AToB_ZeroEdgeCount()
            {
                _graph.AddConnection(1, 2);

                Assert.Equal(0, _graph.EdgeCount);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public async Task AddConnection_AToB_IsCapturableA_IsFalse(int facilityId)
            {
                _graph.AddConnection(1, 2);

                var isCapturable = await _graph.IsFacilityCapturable(facilityId);

                Assert.False(isCapturable);
            }
            #endregion AddConnection
        }

        public class FullMapCompleteGraph : IClassFixture<CompleteMapGraphFixture>
        {
            private readonly CompleteMapGraphFixture _fixture;

            public FullMapCompleteGraph(CompleteMapGraphFixture fixture)
            {
                _fixture = fixture;
            }

            #region Initial State
            [Fact]
            public void InitialState_EdgeCount_IsSeven()
            {
                Assert.Equal(7, _fixture.Graph.EdgeCount);
            }

            [Fact]
            public void InitialState_FacilityCount_IsSeven()
            {
                Assert.Equal(7, _fixture.Graph.FacilityCount);
            }
            #endregion Initial State


            #region IsCapturable
            [Theory]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(6)]
            public async Task IsCapturable_IsTrue(int facilityId)
            {
                var isCapturable = await _fixture.Graph.IsFacilityCapturable(facilityId);

                Assert.True(isCapturable);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(4)]
            [InlineData(7)]
            public async Task IsCapturable_IsFalse(int facilityId)
            {
                var isCapturable = await _fixture.Graph.IsFacilityCapturable(facilityId);

                Assert.False(isCapturable);
            }
            #endregion IsCapturable

            #region GetNeighbors
            [Theory]
            [InlineData(1, 2)]
            [InlineData(2, 3)]
            [InlineData(3, 4)]
            [InlineData(3, 5)]
            [InlineData(3, 6)]
            [InlineData(5, 6)]
            [InlineData(5, 7)]
            [InlineData(2, 1)]
            [InlineData(3, 2)]
            [InlineData(4, 3)]
            [InlineData(5, 3)]
            [InlineData(6, 3)]
            [InlineData(6, 5)]
            [InlineData(7, 5)]
            public void GetNeighbors_ContainsNeighbor(int facilityId, int neighborId)
            {
                var neighbors = _fixture.Graph.GetNeighbors(_fixture.Graph.GetNode(facilityId));

                Assert.Contains(_fixture.Graph.GetNode(neighborId), neighbors);
            }

            [Theory]
            [InlineData(1, 1)]
            [InlineData(2, 2)]
            [InlineData(3, 4)]
            [InlineData(4, 1)]
            [InlineData(5, 3)]
            [InlineData(6, 2)]
            [InlineData(7, 1)]
            public void GetNeighbors_HasNeighborCount(int facilityId, int neighborCount)
            {
                var neighbors = _fixture.Graph.GetNeighbors(_fixture.Graph.GetNode(facilityId));

                Assert.Equal(neighborCount, neighbors.Count());
            }
            #endregion GetNeighbors
        }
    }
}
