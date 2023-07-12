using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace radiants.SpriteScaler
{
	[System.Serializable]
	public class SpriteReactiveProperty : ReactiveProperty<Sprite>
	{
		public SpriteReactiveProperty() : base() { }
		public SpriteReactiveProperty(Sprite value) : base(value) { }
	}

	[System.Serializable]
	public class SpriteDrawModeReactiveProperty : ReactiveProperty<SpriteDrawMode>
	{
		public SpriteDrawModeReactiveProperty() : base() { }
		public SpriteDrawModeReactiveProperty(SpriteDrawMode mode) : base(mode) { }
	}

	[System.Serializable]
	public class SpriteMaskInteractionReactiveProperty : ReactiveProperty<SpriteMaskInteraction>
	{
		public SpriteMaskInteractionReactiveProperty() : base() { }
		public SpriteMaskInteractionReactiveProperty(SpriteMaskInteraction value) : base(value) { }
	}

	[System.Serializable]
	public class SpriteSortPointReactiveProperty : ReactiveProperty<SpriteSortPoint>
	{
		public SpriteSortPointReactiveProperty() : base() { }
		public SpriteSortPointReactiveProperty(SpriteSortPoint point) : base(point) { }
	}

	[System.Serializable]
	public class MaterialReactiveProperty : ReactiveProperty<Material>
	{
		public MaterialReactiveProperty() : base() { }
		public MaterialReactiveProperty(Material value) : base(value) { }
	}


	public static class DefaultSpriteMaterialUtil
	{
		public static Material DefaultSpriteMaterial { get; private set; } = null;

		static DefaultSpriteMaterialUtil()
		{
			var allMats = Resources.FindObjectsOfTypeAll<Material>();

			foreach (var mat in allMats)
			{
				if (mat.name == "Sprites-Default")
				{
					DefaultSpriteMaterial = mat;
					return;
				}
			}
		}

		/* Not work on Unity 2021.x
		#if UNITY_EDITOR
					return UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
		#else
					return Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
		#endif
		*/
	}
}
