using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddBlockCommand : ICommand
{
    private BlockData blockToAdd;
    
    public AddBlockCommand(BlockData block)
    {
        blockToAdd = block;
    }
    
    public void Execute(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var newBlocks = new List<BlockData>(blockInstance.data.blocks ?? new BlockData[0]);
        newBlocks.Add(blockToAdd);
        blockInstance.data.blocks = newBlocks.ToArray();
    }
    
    public void Undo(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var newBlocks = new List<BlockData>(blockInstance.data.blocks);
        newBlocks.RemoveAll(b => b.position == blockToAdd.position);
        blockInstance.data.blocks = newBlocks.ToArray();
    }
}

public class RemoveBlockCommand : ICommand
{
    private BlockData blockToRemove;
    
    public RemoveBlockCommand(BlockData block)
    {
        blockToRemove = block;
    }
    
    public void Execute(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var newBlocks = new List<BlockData>(blockInstance.data.blocks);
        newBlocks.RemoveAll(b => b.position == blockToRemove.position);
        blockInstance.data.blocks = newBlocks.ToArray();
    }
    
    public void Undo(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var newBlocks = new List<BlockData>(blockInstance.data.blocks ?? new BlockData[0]);
        newBlocks.Add(blockToRemove);
        blockInstance.data.blocks = newBlocks.ToArray();
    }
}

public class UpdateBlockFaceCommand : ICommand
{
    private BlockData targetBlock;
    private BlockFace targetFace;
    private string newTexture;
    private string oldTexture;
    
    public UpdateBlockFaceCommand(BlockData block, BlockFace face, string texture)
    {
        targetBlock = block;
        targetFace = face;
        newTexture = texture;
        oldTexture = face.texture;
    }
    
    public void Execute(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        targetFace.texture = newTexture;
    }
    
    public void Undo(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        targetFace.texture = oldTexture;
    }
} 