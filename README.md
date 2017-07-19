# Vtk2Obj
Convert Paraview .Vtk files to Wavefront .Obj files for 3d visualisation in game engines.

Currently supports only conversion of triangulated meshes which must be only a shell (i.e. not a volumetric mesh).

Will process either single or folders of .vtk files.

Suggested usage - create an isosurface across the mesh and export this as a triangulated mesh in .vtk.  

Instruction for ParaView Version 5.0.1

Creation of the geoemtry *.vtk file:
- Open one paraview file
- Unselect all fields in Properties
- Filter - Search - Extract Surface - Enter
- Be sure that the Extract Surface is selected in the Pipeline Browser
- Filter - Search - Generate Surface Normals - Enter
- In Properties: Select "Compute Cell Normals" - Apply
- Be sure that the Generate Surface Normals is selected in the Pipeline Browser
- File - Save Data - Files of type : Legacy VTK Files (*.vtk) - Save
- File type: ASCII - OK

Creation of the *.vtk isosurface files :
- Open paraview files
- Select only the field of interest in Properties
- Filter - Contour
- In Properties: Select "Compute Scalars" and choose the desired value of the isosurface - Apply
- Be sure that Contour is selected in the Pipeline Browser
- Filter - Search - Extract Surface - Enter
- Be sure that the Extract Surface is selected in the Pipeline Browser
- Filter - Search - Generate Surface Normals - Enter
- In Properties: Select "Compute Cell Normals" - Apply
- Be sure that the Generate Surface Normals is selected in the Pipeline Browser
- File - Save Data - Files of type : Legacy VTK Files (*.vtk) - Save
 -Select "Write all timesteps as file-series" - File type: ASCII - OK
