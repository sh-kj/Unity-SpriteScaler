using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace radiants.SpriteScaler
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class SpriteScaler : MonoBehaviour
	{
		#region Properties

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

		public bool preserveAspect
		{
			get { return PreserveAspectReactive.Value; }
			set { PreserveAspectReactive.Value = value; }
		}

		public Color color
		{
			get { return ColorReactive.Value; }
			set { ColorReactive.Value = value; }
		}

		public bool flipX
		{
			get { return TargetRenderer.flipX; }
			set { TargetRenderer.flipX = value; }
		}
		public bool flipY
		{
			get { return TargetRenderer.flipY; }
			set { TargetRenderer.flipY = value; }
		}

		public SpriteDrawMode drawMode
		{
			get { return TargetRenderer.drawMode; }
			set { TargetRenderer.drawMode = value; }
		}

		public SpriteMaskInteraction maskInteraction
		{
			get { return TargetRenderer.maskInteraction; }
			set { TargetRenderer.maskInteraction = value; }
		}

		public SpriteSortPoint sortPoint
		{
			get { return TargetRenderer.spriteSortPoint; }
			set { TargetRenderer.spriteSortPoint = value; }
		}

		public Material material
		{
			get { return TargetRenderer.sharedMaterial; }
			set { TargetRenderer.sharedMaterial = value; }
		}

		public int sortingLayerID
		{
			get { return TargetRenderer.sortingLayerID; }
			set { TargetRenderer.sortingLayerID = value; }
		}

		public int orderInLayer
		{
			get { return TargetRenderer.sortingOrder; }
			set { TargetRenderer.sortingOrder = value; }
		}

		#endregion



		#region references

		private RectTransform _MyRectTransform;
		private RectTransform MyRectTransform
		{ get { if (_MyRectTransform == null) _MyRectTransform = GetComponent<RectTransform>(); return _MyRectTransform; } }

		[SerializeField]
		private SpriteRenderer _TargetRenderer;
		private SpriteRenderer TargetRenderer
		{ get { if (_TargetRenderer == null) CreateAndLinkChildSprite(); return _TargetRenderer; } }
		[SerializeField]
		private Transform _TargetTransform;
		private Transform TargetTransform
		{ get { if (_TargetTransform == null) CreateAndLinkChildSprite(); return _TargetTransform; } }

		#endregion

		#region Make Child Sprite

		private void CreateAndLinkChildSprite()
		{
			GameObject child = new GameObject("_Sprite");
			child.hideFlags = HideFlags.HideAndDontSave;
			_TargetTransform = child.transform;
			child.transform.SetParent(MyRectTransform);
			child.transform.SetSiblingIndex(0);
			child.transform.localRotation = Quaternion.identity;
			child.layer = gameObject.layer;
			_TargetRenderer = child.AddComponent<SpriteRenderer>();
		}

		#endregion

		#region Observables

		[SerializeField]
		private BoolReactiveProperty PreserveAspectReactive = new BoolReactiveProperty(false);

		[SerializeField]
		private ColorReactiveProperty ColorReactive = new ColorReactiveProperty(UnityEngine.Color.white);

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
			TargetRenderer.enabled = true;
			//ResearchParentGroups();

			RectTransformDimensionsChangedSubject
				.Where(_ => TargetRenderer.sprite != null)
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(disposables);

			PreserveAspectReactive
				.Where(_ => TargetRenderer.sprite != null)
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(disposables);

			ColorReactive.Subscribe(_ => UpdateColor())
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
			Destroy(TargetRenderer.gameObject);
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
				if (PreserveAspectReactive.Value)
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


		private void UpdateColor()
		{
			Color c = ColorReactive.Value;
			TargetRenderer.color = c;
		}

		#endregion

	}

}