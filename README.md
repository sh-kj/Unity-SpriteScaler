# Unity Sprite Scaler

SpriteRenderer scalable with RectTransform.

## Requirement

This library requires UniRx or R3.

## Install via git URL

for R3: https://github.com/sh-kj/Unity-SpriteScaler.git?path=R3/SpriteScaler  
for UniRx: https://github.com/sh-kj/Unity-SpriteScaler.git?path=UniRx/SpriteScaler  

## Usage

Add SpriteScaler component, and set Sprite in Inspector.  
This behaves like `UnityEngine.UI.Image` but less performance overhead.

## Note

- This doesn't react with `Graphic Raycaster`, but `Physics 2D Raycaster` is useful and good performance.

- Hierarchy doesn't affect to Sorting order, so you have to set `Sorting Order` and `Order In Layer` manually.

- `Canvas Group`'s alpha doesn't affect to this.

- If you want to change material, your shader would be based on `Sprites/Default`, not `UI/Default`.

## License
MIT