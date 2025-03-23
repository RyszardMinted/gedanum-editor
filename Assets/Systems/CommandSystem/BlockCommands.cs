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
    private string faceName;
    private string newTexture;
    private string oldTexture;
    
    public UpdateBlockFaceCommand(BlockData block, BlockFace face, string texture)
    {
        targetBlock = block;
        faceName = GetFaceName(block, face);
        newTexture = texture;
        oldTexture = face.texture;
    }
    
    public void Execute(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var face = GetFaceFromName(targetBlock, faceName);
        if (face != null)
        {
            face.texture = newTexture;
        }
    }
    
    public void Undo(BlockInstance blockInstance)
    {
        if (blockInstance == null || blockInstance.data == null) return;
        
        var face = GetFaceFromName(targetBlock, faceName);
        if (face != null)
        {
            face.texture = oldTexture;
        }
    }

    private string GetFaceName(BlockData block, BlockFace face)
    {
        if (face == block.top) return "top";
        if (face == block.bottom) return "bottom";
        if (face == block.front) return "front";
        if (face == block.back) return "back";
        if (face == block.left) return "left";
        if (face == block.right) return "right";
        return null;
    }

    private BlockFace GetFaceFromName(BlockData block, string faceName)
    {
        return faceName switch
        {
            "top" => block.top,
            "bottom" => block.bottom,
            "front" => block.front,
            "back" => block.back,
            "left" => block.left,
            "right" => block.right,
            _ => null
        };
    }
} 