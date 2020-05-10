using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace radiants.SpriteScaler
{
	//todo custom Material
	//note: Mask関連は要ドキュメント化

	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class SpriteScaler : MonoBehaviour
	{
		public Sprite RenderingSprite
		{
			get
			{
				if (TargetRenderer == null) return null;
				return TargetRenderer.sprite;
			}
			set
			{
				if (TargetRenderer == null) return;
				TargetRenderer.sprite = value;
				if (value != null)
					AdjustSpriteScale();
			}
		}


		#region references

		private RectTransform _MyRectTransform;
		public RectTransform MyRectTransform
		{ get { if (_MyRectTransform == null) _MyRectTransform = GetComponent<RectTransform>(); return _MyRectTransform; } }

		[SerializeField]
		public SpriteRenderer _TargetRenderer;
		public SpriteRenderer TargetRenderer
		{ get { return _TargetRenderer; } }
		[SerializeField]
		public Transform _TargetTransform;
		public Transform TargetTransform
		{ get { return _TargetTransform; } }

		#endregion

		#region Observables

		[SerializeField]
		public BoolReactiveProperty PreserveAspect = new BoolReactiveProperty(false);

		[SerializeField]
		public ColorReactiveProperty color = new ColorReactiveProperty(Color.white);

		private Subject<Unit> RectTransformDimensionsChangedSubject = new Subject<Unit>();

		private CompositeDisposable disposables = new CompositeDisposable();

		#endregion

		#region Monobehaviour callback

		private void OnRectTransformDimensionsChange()
		{
			RectTransformDimensionsChangedSubject.OnNext(Unit.Default);
		}

		private void OnEnable()
		{
			if (TargetRenderer == null) return;

			TargetRenderer.enabled = true;
			//ResearchParentGroups();

			RectTransformDimensionsChangedSubject
				.Where(_ => TargetRenderer.sprite != null)
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(disposables);

			PreserveAspect
				.Where(_ => TargetRenderer.sprite != null)
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(disposables);

			color.Subscribe(_ => UpdateColor())
				.AddTo(disposables);
		}

		private void OnDisable()
		{
			if (TargetRenderer != null)
				TargetRenderer.enabled = false;

			disposables.Clear();
		}

		private void OnDestroy()
		{
			disposables.Dispose();
		}


		#endregion

		#region Modify Scales

		public void AdjustSpriteScale()
		{
			var originalBounds = TargetRenderer.sprite.bounds.size;
			Vector3 scale;

			if (TargetRenderer.drawMode == SpriteDrawMode.Sliced
				|| TargetRenderer.drawMode == SpriteDrawMode.Tiled)
			{
				//9-sliced
				scale = new Vector3(TargetRenderer.sprite.pixelsPerUnit, TargetRenderer.sprite.pixelsPerUnit, 1f);
				TargetRenderer.size = MyRectTransform.rect.size / TargetRenderer.sprite.pixelsPerUnit;
			}
			else
			{
				//normal
				scale = new Vector3(MyRectTransform.rect.size.x / originalBounds.x, MyRectTransform.rect.size.y / originalBounds.y, 1f);
				if (PreserveAspect.Value)
					PreserveAspectScale(ref scale);
			}

			TargetTransform.localScale = scale;
			TargetTransform.localPosition = MyRectTransform.rect.center;
		}

		private void PreserveAspectScale(ref Vector3 _scale)
		{
			if (_scale.x > _scale.y)
			{
				_scale.x = _scale.y;
			}
			else
			{
				_scale.y = _scale.x;
			}
		}

		#endregion

		#region Modify Colors

		//SpriteGroup[] Groups;
		/*
		public void ResearchParentGroups()
		{
			Groups = GetComponentsInParent<SpriteGroup>();
		}
		*/

		public void UpdateAlpha()
		{
			if (TargetRenderer != null)
				UpdateColor();
		}


		private float CalcGroupsAlpha()
		{
			return 1f;
			/*
			float ret = 1f;

			for (int i = Groups.Length - 1; i >= 0; i--)
			{
				if (Groups[i] != null && !Groups[i].enabled) continue;
				ret *= Groups[i].Alpha.Value;
			}

			return ret;
			*/
		}

		private void UpdateColor()
		{
			Color c = color.Value;
			c.a *= CalcGroupsAlpha();
			TargetRenderer.color = c;
		}

		#endregion

	}
}