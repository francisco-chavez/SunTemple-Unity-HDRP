# SunTemple-Unity-HDRP

ORCA UE4 Sun Temple v3 HDRP with Shader Graph

Adapts a copy of version 3 of the UE4 Sun Temple assets from the NVIDIA's ORCA Project, which can be found at https://developer.nvidia.com/orca.

Simply put, the assets aren't well formated for Unity, and I'm trying to change that. In this specific repo, I'm adapting the assets to Unity 2018.4 and HDRP. I used Unity's Shader Graph to create shaders that work with the supplied textures. I wrote a basic fscene reader to pull the lighting inforation (it doesn't do everything that's needed, but it does save a lot of time). And, I'm using the embeded prefab system to cut down on the amount of resoures that are being wasted on identical meshes.

ORCA has some really nice assets, and I think that it's a shame that they don't work with Unity right out of the box.
