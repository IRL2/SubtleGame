# Narupa Unity Plugin

A set of libraries for creating Narupa applications in Unity.

This repository is automatically updated with the latest state of the libraries
in the [Narupa iMD](https://gitlab.com/intangiblerealities/narupa-applications/narupa-imd) 
application, which represents the latest state of the Narupa libraries for Unity.

The easiest way to use these libraries is to add them as a submodule to your 
Unity3D project.

```
cd my-narupa-unity-project
mkdir -p Assets/Plugins 
git submodule add git@gitlab.com:intangiblerealities/narupa-unity-plugin.git Assets/Plugins/Narupa
git submodule update --init 
```

Whenever you need to update the Narupa plugin, you can just run

```
git submodule update
```

Any useful modifications or changes to the Narupa libraries should be submitted
as a merge request on the [Narupa iMD](https://gitlab.com/intangiblerealities/narupa-applications/narupa-imd) 
repository. 