using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public GameObject Floor { get; set; }
    public GameObject Obj { get; set; }

    public GridCell()
    {
        Floor = null;
        Obj = null;
    }
}
