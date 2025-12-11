namespace Core
{
    public interface IPuzzleRule
    {
        bool TryPlacePiece(string pieceId, string targetId);
        bool IsCompleted { get; }
    }
}