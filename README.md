# Vtk2Obj
Convert Paraview .Vtk files to Wavefront .Obj files for 3d visualisation in game engines.

Currently supports only conversion of triangulated meshes which must be only a shell (i.e. not a volumetric mesh).

Will process either single or folders of .vtk files.

Suggested usage - create an isosurface across the mesh and export this as a triangulated mesh in .vtk.  