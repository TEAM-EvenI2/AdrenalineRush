using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemGenerator : MonoBehaviour
{
	public abstract void GenerateItems(MapMeshWrapper pipe);
}
