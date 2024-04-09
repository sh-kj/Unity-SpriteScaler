using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace radiants.SpriteScaler
{	
	[CustomEditor(typeof(SpriteScaler))]
	public class SpriteScalerEditor : Editor
	{
		private SpriteScaler _Target;
		private SpriteScaler Target
		{
			get
			{
				if (_Target == null) _Target = target as SpriteScaler;
				return _Target;
			}
		}

		public override void OnInspectorGUI()
		{
			var preserveAspect = Target.preserveAspect;
			var sprite = Target.renderSprite;
			var color = Target.color;
			bool flipX = Target.flipX, flipY = Target.flipY;
			var drawMode = Target.drawMode;
			var scaleForTiled = Target.ScaleForTiledAndSliced;
			var maskInteraction = Target.maskInteraction;
			var sortPoint = Target.sortPoint;
			var material = Target.customMaterial;
			var sortingLayerID = Target.sortingLayerID;
			var orderInLayer = Target.orderInLayer;


			EditorGUI.BeginChangeCheck();

			sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);
			preserveAspect = EditorGUILayout.Toggle("Preserve Aspect", preserveAspect);
			color = EditorGUILayout.ColorField("Color", color);
			flipX = EditorGUILayout.Toggle("Flip X", flipX);
			flipY = EditorGUILayout.Toggle("Flip Y", flipY);
			drawMode = (SpriteDrawMode)EditorGUILayout.EnumPopup("Draw Mode", drawMode);
			if(drawMode != SpriteDrawMode.Simple)
			{
				scaleForTiled = EditorGUILayout.FloatField("Scale for Tiled/Sliced", scaleForTiled);
			}
			maskInteraction = (SpriteMaskInteraction)EditorGUILayout.EnumPopup("Mask Interaction", maskInteraction);
			sortPoint = (SpriteSortPoint)EditorGUILayout.EnumPopup("Sprite Sort Point", sortPoint);
			material = (Material)EditorGUILayout.ObjectField("Custom Material", material, typeof(Material), false);

			var sortingLayerIndex = GetSortingLayerIndex(sortingLayerID);
			if (sortingLayerIndex == -1) sortingLayerIndex = 0;
			sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, GetSortingLayerNames());
			sortingLayerID = SortingLayer.layers[sortingLayerIndex].id;

			orderInLayer = EditorGUILayout.IntField("Order In Layer", orderInLayer);


			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(Target, "Modify SpriteScaler");

				Target.preserveAspect = preserveAspect;
				Target.renderSprite = sprite;
				Target.color = color;
				Target.flipX = flipX;
				Target.flipY = flipY;
				Target.drawMode = drawMode;
				Target.ScaleForTiledAndSliced = scaleForTiled;
				Target.maskInteraction = maskInteraction;
				Target.sortPoint = sortPoint;
				Target.customMaterial = material;
				Target.sortingLayerID = sortingLayerID;
				Target.orderInLayer = orderInLayer;
			}

			if(GUI.changed)
				EditorUtility.SetDirty(Target);
		}

		private static int GetSortingLayerIndex(int id)
		{
			for (int i = 0; i < SortingLayer.layers.Length; i++)
			{
				if (SortingLayer.layers[i].id == id)
					return i;
			}
			return -1;
		}
		private static string[] GetSortingLayerNames()
		{
			return SortingLayer.layers.Select(_layer => _layer.name).ToArray();
		}
	}
}

