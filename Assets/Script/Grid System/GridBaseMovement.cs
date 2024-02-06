using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBaseMovement : MonoBehaviour
{
    public float moveSpeed;
    public Transform movePoint;

    public GridData gridsData;
    public ObjectsDatabaseSO dataBase;
    
    private void Start()
    {
        movePoint.parent = null;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,movePoint.position) <= 0.05f)
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
            
            if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                movePoint.position += new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
            }
        }

        /*if(!gridsData.CanPlaceObejctAt(Vector3Int.FloorToInt(transform.position), dataBase.objectsData[0].Size))
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }*/
    }
}
