using UnityEngine;

[System.Serializable]
public class StandardBlocks {
    public string blockName;                  

    public Vector3Int size;           
    public BlockData[] blocks;       
}

[System.Serializable]
public class BlockData {
    public Vector3Int position;         
    public BlockFace top;              
    public BlockFace bottom;         
    public BlockFace front;            
    public BlockFace back;              
    public BlockFace left;            
    public BlockFace right;            
}

[System.Serializable]
public class BlockFace
{
    public string texture;
    public Vector4 uv;
}
