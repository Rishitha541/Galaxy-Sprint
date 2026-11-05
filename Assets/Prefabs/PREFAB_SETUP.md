# Prefab Setup Guide

This game uses simple low-poly prefabs. The editor builder can create starter prefabs automatically from `Tools > Endless Runner > Build Complete Demo Scene`, or you can build them manually using the guide below and replace them later with Blender assets.

## RoadTile Prefab

1. Create a Cube.
2. Rename it `RoadTile`.
3. Set scale:

```text
X: 10
Y: 0.2
Z: 30
```

4. Place pivot at center.
5. Add a low-poly material, for example dark asphalt or stylized stone.
6. Convert to prefab.
7. Pool ID: `RoadTile`.

Optional:

- Add lane markings using thin white cubes.
- Add side rails or small curb cubes.

## Coin Prefab

1. Create a Cylinder.
2. Rename it `Coin`.
3. Set scale:

```text
X: 0.6
Y: 0.08
Z: 0.6
```

4. Rotate cylinder if you want it to face the camera.
5. Add a gold material.
6. Add a Collider and enable `Is Trigger`.
7. Add `CoinSystem.cs`.
8. Set tag to `Coin`.
9. Convert to prefab.
10. Pool ID: `Coin`.

## Barrier Prefab

1. Create a Cube.
2. Rename it `Barrier`.
3. Set scale:

```text
X: 2.2
Y: 1.2
Z: 0.5
```

4. Add Collider.
5. Add `Obstacle.cs`.
6. Set tag to `Obstacle`.
7. Convert to prefab.
8. Pool ID: `Barrier`.

## Box Prefab

1. Create a Cube.
2. Rename it `Box`.
3. Set scale:

```text
X: 1.4
Y: 1.4
Z: 1.4
```

4. Add Collider.
5. Add `Obstacle.cs`.
6. Set tag to `Obstacle`.
7. Convert to prefab.
8. Pool ID: `Box`.

## Wall Prefab

1. Create a Cube.
2. Rename it `Wall`.
3. Set scale:

```text
X: 3
Y: 2.5
Z: 0.7
```

4. Add Collider.
5. Add `Obstacle.cs`.
6. Set tag to `Obstacle`.
7. Convert to prefab.
8. Pool ID: `Wall`.

## Tree Prefab

Quick low-poly tree:

1. Add a Cylinder for trunk.
2. Add a Cone or low-poly sphere for leaves.
3. Parent both under an empty GameObject named `Tree`.
4. Convert parent to prefab.
5. Pool ID: `Tree`.

## Rock Prefab

Quick low-poly rock:

1. Create a low-poly sphere or cube.
2. Scale non-uniformly.
3. Add gray/brown material.
4. Convert to prefab.
5. Pool ID: `Rock`.

## Performance Notes

- Keep decorative prefabs low-poly.
- Use shared materials.
- Do not add Rigidbody unless needed.
- Obstacles only need colliders.
- Coins should use trigger colliders.
- All spawned objects should come from `ObjectPool`, not manual runtime instantiation.
