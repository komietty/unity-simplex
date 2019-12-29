# unity-simplex-geometry

Simplex geometry utility library on 2D/3D. If you are looking for some circumCircle/circumSphere method on 2D/3D, this will help you.

![img](Assets/Demo/Demo.PNG)

## Usage

This library is based on Unity.Mathematics SIMD library, be sure to install them beforehand (ver 1.1.0).

If you want to just use modules, type below.
```
git submodule add git@github.com:komietty/unity-simplex-geometry.git Assets/Packages/unity-simplex-geometry
git commit -m "add module"
cd Assets/Packages/unity-simplex-geometry
git config core.sparsecheckout true
echo "Assets/D2" > ../../../.git/modules/Assets/Packages/unity-simplex-geometry/info/sparse-checkout
echo "Assets/D3" >> ../../../.git/modules/Assets/Packages/unity-simplex-geometry/info/sparse-checkout
git read-tree -m -u HEAD
```

## License
[MIT](LICENSE)
