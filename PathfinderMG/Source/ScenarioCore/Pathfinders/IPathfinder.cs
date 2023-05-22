using System;
using System.Threading;
using System.Threading.Tasks;

namespace PathfinderMG.Core.Source.ScenarioCore.Pathfinders;

internal interface IPathfinder
{
    bool AllowDiagonalMovement { get; set; }
    bool InstantPathing { get; set; }
    int VisualizationTime { get; set; }

    event EventHandler PathfindingFinished;

    /// <summary>
    /// Every <see cref="IPathfinder"/> should accept a <see cref="Grid"/> object in order to manipulate it during pathfinding.
    /// </summary>
    void SetupGrid(Grid grid, int visualizationTime);
    Task FindPathAsync(CancellationToken cancellationToken);
    void ClearPath();
}
