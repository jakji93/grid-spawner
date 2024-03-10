using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class GridSpawner : EditorWindow
{
   private GameObject prefabToSpawn;
   private GameObject parentObject;
   private float cellWidth = 100;
   private float cellHeight = 100;
   private int gridWidth = 1;
   private int gridHeight = 1;
   private int[,] dataList = new int[1,1];
   private int tempWidth = 1, tempHeight = 1;
   private Vector2 scrollPosition = Vector2.zero;

   [MenuItem("Tools/Grid Spawner")]
   public static void ShowWindow()
   {
      GetWindow<GridSpawner>("Grid Spawner");
   }

   private void OnGUI()
   {
      prefabToSpawn = EditorGUILayout.ObjectField("Object to Spawn:", prefabToSpawn, typeof(GameObject), false) as GameObject;
      parentObject = EditorGUILayout.ObjectField("Parent Object:", parentObject, typeof(GameObject), true) as GameObject;
      cellWidth = EditorGUILayout.FloatField("Cell Width:", cellWidth);
      cellHeight = EditorGUILayout.FloatField("Cell Height:", cellHeight);
      gridWidth = EditorGUILayout.IntField("Grid Width:", gridWidth);
      string buttonID = "";
      if (gridWidth < 1) {
         gridWidth = 1;
      }
      gridHeight = EditorGUILayout.IntField("Grid Height:", gridHeight);
      if (gridHeight < 1) {
         gridHeight = 1;
      }
      if(gridWidth != tempWidth || gridHeight != tempHeight) {
         tempWidth = gridWidth;
         tempHeight = gridHeight;
         dataList = new int[gridHeight,gridWidth];
      }
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      for (int row = gridHeight-1; row >=0; row--) {
         GUILayout.BeginHorizontal();
         for (int col = 0; col < gridWidth; col++) {
            Color originalBackgroundColor = GUI.backgroundColor;
            buttonID = row.ToSafeString() + "-" + col.ToSafeString();
            if (dataList[row, col] == 1) {
               GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button(buttonID, GUILayout.Height(50), GUILayout.Width(50))) {
               dataList[row, col] = dataList[row, col] == 0 ? 1 : 0;
            }
            GUI.backgroundColor = originalBackgroundColor;
            buttonID = "";
         }
         GUILayout.EndHorizontal();
      }
      EditorGUILayout.EndScrollView();
      GUILayout.Space(20);
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Create Grid", GUILayout.Height(30))) {
         DeleteSpawnedObjects();
         SpawnNotes();
      }
      if (GUILayout.Button("Reset Grid", GUILayout.Height(30))) {
         for (int row = 0; row < gridHeight; row++) {
            for (int col = 0; col < gridWidth; col++) {
               dataList[row, col] = 0;
            }
         }
      }
      if (GUILayout.Button("Scan Grid", GUILayout.Height(30))) {
         ScanNotes();
      }
      GUILayout.EndHorizontal();
   }

   private void SpawnNotes()
   {
      for (int row = 0; row < gridHeight; row++) {
         for (int col = 0; col < gridWidth; col++) {
            if (dataList[row, col] == 1) {
               GameObject newObject = PrefabUtility.InstantiatePrefab(prefabToSpawn) as GameObject;
               newObject.transform.SetParent(parentObject.transform);
               newObject.transform.localPosition = new Vector3(col * cellWidth + cellWidth/2, row * cellHeight + cellHeight / 2, 0f);
               Undo.RegisterCreatedObjectUndo(newObject, "Spawned Object");
            }
         }
      }
   }

   private void DeleteSpawnedObjects()
   {
      if (parentObject != null) {
         for (int i = parentObject.transform.childCount - 1; i >= 0; i--) {
            Undo.DestroyObjectImmediate(parentObject.transform.GetChild(i).gameObject);
         }
      }
   }

   private void ScanNotes()
   {
      for (int row = 0; row < gridHeight; row++) {
         for (int col = 0; col < gridWidth; col++) {
            dataList[row, col] = 0;
         }
      }
      for (int i = 0; i < parentObject.transform.childCount; ++i) {
         var position = parentObject.transform.GetChild(i).position;
         var row = Mathf.FloorToInt((position.y - cellHeight / 2) / cellHeight);
         var col = Mathf.FloorToInt((position.x - cellWidth / 2) / cellWidth);
         dataList[row, col] = 1;
      }
   }
}
