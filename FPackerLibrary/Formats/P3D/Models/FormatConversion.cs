using FPackerLibrary.P3D.Math;
using FPackerLibrary.P3D.Models.MLOD;
using FPackerLibrary.P3D.Models.MLOD.Tagg;
using FPackerLibrary.P3D.Models.ODOL;

namespace FPackerLibrary.P3D.Models; 

public static class FormatConversion {

    public static MLOD.MLOD Covert2MLOD(ODOL.ODOL odol) {
        var lodArr = odol.LODs;
        var num = lodArr.Length;
        var array = new MLOD_LOD[num];
        for (var i = 0; i < num; i++) array[i] = OdolLod2MLOD(odol, (LOD)lodArr[i]);
        return new MLOD.MLOD( array);
    }

    public static MLOD_LOD OdolLod2MLOD(ODOL.ODOL odolObj, LOD src) {
        var output = new MLOD_LOD(src.Resolution);
        var vertexCount = src.VertexCount;
        var mass = odolObj.ModelInfo.Mass;
        var skeleton = odolObj.Skeleton;
        
        ConvertPoints(odolObj, output, src);
        output.Normals = src.Normals;
        ConvertFaces(output, src);
        output.Taggs = new List<Tagg>();
        if (System.Math.Abs(src.Resolution - 1E+13f) < 0.000001) output.Taggs.Add(CreateMassTagg(vertexCount, mass));
        
        output.Taggs.AddRange(CreateUVSetTaggs(src));
        output.Taggs.AddRange(CreatePropertyTaggs(src));
        output.Taggs.AddRange(CreateNamedSelectionTaggs(src));
        output.Taggs.AddRange(CreateAnimTaggs(src));
        
        if (Resolution.KeepsNamedSelections(src.Resolution)) return output;

        ReconstructNamedSelectionBySections(src, out var nsPoints, out var nsFaces);
        ReconstructProxies(src, out var nsPoints2, out var nsFaces2);
        ReconstructNamedSelectionsByBones(src, odolObj.Skeleton, out var nsPoints3);    
        ApplySelectedPointsAndFaces(output, nsPoints, nsFaces);
        ApplySelectedPointsAndFaces(output, nsPoints2, nsFaces2);
        ApplySelectedPointsAndFaces(output, nsPoints3, null);
        
        return output;
    }
    
    private static void ApplySelectedPointsAndFaces(MLOD_LOD dstLod, IReadOnlyDictionary<string, List<PointWeight>> nsPoints, IReadOnlyDictionary<string, List<int>>? nsFaces) {
        foreach (var namedSelectionTagg in dstLod.Taggs.OfType<NamedSelectionTagg>().Select(static tagg => tagg)) {
            if (nsPoints != null && nsPoints.TryGetValue(namedSelectionTagg.Name, out var list)) {
                foreach (var pointWeight in list) {
                    var b = (byte)-pointWeight.Weight;
                    if (b != 0) namedSelectionTagg.Points[pointWeight.PointIndex] = b;
                }
            }

            if (nsFaces == null || !nsFaces.TryGetValue(namedSelectionTagg.Name, out var list2)) continue;
            foreach (var num in list2) namedSelectionTagg.Faces[num] = 1;
        }
    }
    
    private static void ReconstructNamedSelectionsByBones(LOD src, Skeleton skeleton, out Dictionary<string, List<PointWeight>> points) {
        points = new Dictionary<string, List<PointWeight>>(src.NamedSelections.Length * 2);
        if (src.VertexBoneRef.Length == 0) return;
        ushort num = 0;
        var vertexBoneRef = src.VertexBoneRef;
        foreach (var t in vertexBoneRef) {
            foreach (var animationRTPair in t.AnimationRTPairs) {
                byte selectionIndex = animationRTPair.SelectionIndex, weight = animationRTPair.Weight;  
                var num2 = src.SubSkeletonsToSkeleton[selectionIndex];
                var key = skeleton.Bones[num2 * 2];
                var item = new PointWeight(num, weight);
                if (!points.TryGetValue(key, out var list)) {
                    list = new List<PointWeight>(10000) {item};
                    points[key] = list;
                } else list.Add(item);
            }
            num += 1;
        }
    }
    
    private static void ReconstructProxies(LOD src, out Dictionary<string, List<PointWeight>> points, out Dictionary<string, List<int>> faces) {
			points = new Dictionary<string, List<PointWeight>>(src.NamedSelections.Length * 2);
			faces = new Dictionary<string, List<int>>(src.NamedSelections.Length * 2);
			for (var i = 0; i < src.Faces.Length; i++) {
				var polygon = src.Faces[i];
                if (polygon.VertexIndices.Length != 3) continue;
                
                VertexIndex vi = polygon.VertexIndices[0], vi2 = polygon.VertexIndices[1], vi3 = polygon.VertexIndices[2];
                Vector3P vector3P = src.Vertices[vi], vector3P2 = src.Vertices[vi2], vector3P3 = src.Vertices[vi3];
                float num = vector3P.Distance(vector3P2), num2 = vector3P.Distance(vector3P3), num3 = vector3P2.Distance(vector3P3);
                
                if (num > num2) {
                    Functions.Swap(ref vector3P2, ref vector3P3);
                    Functions.Swap(ref num, ref num2);
                }
                if (num > num3) {
                    Functions.Swap(ref vector3P, ref vector3P3);
                    Functions.Swap(ref num, ref num3);
                }
                if (num2 > num3) {
                    Functions.Swap(ref vector3P, ref vector3P2);
                    Functions.Swap(ref num2, ref num3);
                }
                Vector3P 
                    vector3P4 = vector3P, 
                    vector3P5 = vector3P2 - vector3P, 
                    vector3P6 = vector3P3 - vector3P;
                vector3P5.Normalize();
                vector3P6.Normalize();
                if (!Functions.EqualsFloat(vector3P6 * vector3P5, 0f, 0.05f)) continue;
                
                foreach (var t in src.Proxies) {
                    Vector3P 
                        position = t.Transformation.Position, 
                        up = t.Transformation.Orientation.Up,
                        dir = t.Transformation.Orientation.Dir;
                    if (!vector3P4.Equals(position) || !vector3P5.Equals(dir) || !vector3P6.Equals(up)) continue;
                    var name = src.NamedSelections[t.NamedSelectionIndex].Name;
                    if (faces.ContainsKey(name)) continue;
                    faces[name] = i.Yield().ToList();
                    points[name] = Functions.Yield(new PointWeight[] {
                        new(vi, byte.MaxValue),
                        new(vi2, byte.MaxValue),
                        new(vi3, byte.MaxValue)
                    }).ToList();
                    break;
                }
            }
		}
    
    private static IEnumerable<AnimationTagg> CreateAnimTaggs(LOD src) {
        foreach (var keyframe in src.Frames) {
            var num = keyframe.Points.Length;
            var animationTagg = new AnimationTagg {
                Name = "#Animation#",
                DataSize = (uint)(num * 12 + 4),
                FrameTime = keyframe.Time,
                FramePoints = new Vector3P[num]
            };
            Array.Copy( keyframe.Points, animationTagg.FramePoints, num );
            yield return animationTagg;
        }
    }
    
    private static IEnumerable<NamedSelectionTagg> CreateNamedSelectionTaggs(LOD src) {
        int nPoints = src.VertexCount, nFaces = src.Faces.Length;
        var namedSelections = src.NamedSelections;
        foreach (var namedSelection in namedSelections)
        {
            var namedSelectionTagg = new NamedSelectionTagg {
                Name = namedSelection.Name,
                DataSize = (uint)(nPoints + nFaces),
                Points = new byte[nPoints],
                Faces = new byte[nFaces]
            };
            var flag = namedSelection.SelectedVerticesWeights.Length != 0;
            var num = 0;
            var selectedVertices = namedSelection.SelectedVertices;
            foreach (int num2 in selectedVertices) {
                var b = (byte)((!flag) ? 1 : ((byte)(-namedSelection.SelectedVerticesWeights[num++])));
                namedSelectionTagg.Points[num2] = b;
            }
            selectedVertices = namedSelection.SelectedFaces;
            foreach (int num3 in selectedVertices) {
                if ( num3 < 0 ) continue;
                namedSelectionTagg.Faces[num3] = 1;
            }
            yield return namedSelectionTagg;
        }
    }
    
    private static PointFlags ClipFlagsToPointFlags(ClipFlags clipFlags) {
        var pointFlags = PointFlags.NONE;
        if ((clipFlags & ClipFlags.ClipLandStep) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.ONLAND;
        } else if ((clipFlags & ClipFlags.ClipLandUnder) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.UNDERLAND;
        } else if ((clipFlags & ClipFlags.ClipLandAbove) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.ABOVELAND;
        } else if ((clipFlags & ClipFlags.ClipLandKeep) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.KEEPLAND;
        } 
        
        if ((clipFlags & ClipFlags.ClipDecalStep) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.DECAL;
        } else if ((clipFlags & ClipFlags.ClipDecalVertical) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.VDECAL;
        }
        
        if ((clipFlags & (ClipFlags)209715200) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.NOLIGHT;
        } else if ((clipFlags & (ClipFlags)212860928) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.FULLLIGHT;
        } else if ((clipFlags & (ClipFlags)211812352) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.HALFLIGHT;
        } else if ((clipFlags & (ClipFlags)210763776) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.AMBIENT;
        } 
        
        if ((clipFlags & ClipFlags.ClipFogStep) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.NOFOG;
        } else if ((clipFlags & ClipFlags.ClipFogSky) != ClipFlags.ClipNone) {
            pointFlags |= PointFlags.SKYFOG;
        }
        
        var num = (int)(clipFlags & ClipFlags.ClipUserMask) / (int)ClipFlags.ClipUserStep;
        return pointFlags | (PointFlags)(65536 * num);
    }

    private static MassTagg CreateMassTagg(int nPoints, float totalMass) {
        var massTagg = new MassTagg {
            Name = "#Mass#",
            DataSize = (uint)(nPoints * 4),
            Mass = new float[nPoints]
        };
        
        for (var i = 0; i < nPoints; i++)  massTagg.Mass[i] = totalMass / nPoints;
        
        return massTagg;
    }
    
    private static IEnumerable<UVSetTagg> CreateUVSetTaggs(LOD src) {
        var nFaces = src.Faces.Length;
        int num3;
        for (var i = 0; i < src.UVSets.Length; i = num3) {
            var uvSetTagg = new UVSetTagg {
                Name = "#UVSet#",
                UVSetNr = (uint)i,
                FaceUVs = new float[nFaces][,]
            };
            var uvData = src.UVSets[i].UVData;
            var num = 4U;
            for (var j = 0; j < nFaces; j++) {
                var polygon = src.Faces[j];
                var num2 = polygon.VertexIndices.Length;
                uvSetTagg.FaceUVs[j] = new float[num2, 2];
                for (var k = 0; k < num2; k++) {
                    var vi = polygon.VertexIndices[num2 - 1 - k];
                    uvSetTagg.FaceUVs[j][k, 0] = uvData[vi * 2];
                    uvSetTagg.FaceUVs[j][k, 1] = uvData[vi * 2 + 1];
                    num += 8U;
                }
            }
            uvSetTagg.DataSize = num;
            yield return uvSetTagg;
            num3 = i + 1;
        }
        yield break;
    }

    private static void ConvertPoints(ODOL.ODOL odolObj, MLOD_LOD dstLod, LOD srcLod) {
        Vector3P boundingCenter = odolObj.ModelInfo.BoundingCenter, bboxMinVisual = odolObj.ModelInfo.BboxMinVisual,
        bboxMaxVisual = odolObj.ModelInfo.BboxMaxVisual;
        var num = srcLod.Vertices.Length;
        dstLod.Points = new Point[num];
        
        for (var i = 0; i < num; i++) {
            var pos = srcLod.Vertices[i] + boundingCenter;
            dstLod.Points[i] = new Point(pos, ClipFlagsToPointFlags(srcLod.ClipFlags[i]));
        }
    }
    
    private static void ReconstructNamedSelectionBySections(LOD src, out Dictionary<string, List<PointWeight>> points, out Dictionary<string, List<int>> faces) {
        points = new Dictionary<string, List<PointWeight>>( src.NamedSelections.Length * 2 );
        faces = new Dictionary<string, List<int>>( src.NamedSelections.Length * 2 );
        var namedSelections = src.NamedSelections;
        foreach (var namedSelection in namedSelections) {
            if (!namedSelection.IsSectional) continue;
            var enumerable = namedSelection.Sections.SelectMany(si => src.Sections[si].getFaceIndexes(src.Faces));
            var enumerable2 = enumerable.SelectMany(fi => src.Faces[fi].VertexIndices)
                .Select(static vi => new PointWeight(vi, byte.MaxValue));
            faces[namedSelection.Name] = enumerable.Select(static fi => (int)fi ).ToList();
            points[namedSelection.Name] = enumerable2.ToList();
        }
    }
    
    private static void ConvertFaces(MLOD_LOD dstLod, LOD srcLod) {
        var list = new List<Face>(srcLod.VertexCount * 2);
        foreach (var section in srcLod.Sections) {
            var uvData = srcLod.UVSets[0].UVData;
            foreach (var num in section.getFaceIndexes(srcLod.Faces)) {
                var num2 = srcLod.Faces[(int)num].VertexIndices.Length;
                var array = new Vertex[num2];
                for (var k = 0; k < num2; k++) {
                    int num3 = srcLod.Faces[(int)num].VertexIndices[num2 - 1 - k];
                    array[k] = new Vertex(num3, num3, uvData[num3 * 2], uvData[num3 * 2 + 1]);
                }
                var texture = (section.TextureIndex == -1) ? "" : srcLod.Textures[section.TextureIndex];
                var material = (section.MaterialIndex == -1) ? "" : srcLod.Materials[section.MaterialIndex].MaterialName;
                var item = new Face(num2, array, FaceFlags.DEFAULT, texture, material);
                list.Add(item);
            }
        }
        dstLod.Faces = list.ToArray();
    }
    
    private static IEnumerable<PropertyTagg> CreatePropertyTaggs(LOD src) {
        int num;
        for (var i = 0; i < src.NamedProperties.Length / 2; i = num) {
            yield return new PropertyTagg {
                Name = "#Property#",
                DataSize = 128U,
                PropertyName = src.NamedProperties[i, 0],
                PropertyValue = src.NamedProperties[i, 1]
            };
            num = i + 1;
        }
        yield break;
    }

    private struct PointWeight {
        public int PointIndex;
        public byte Weight;
        
        public PointWeight(int index, byte weight) {
            PointIndex = index;
            Weight = weight;
        }

    }
}