using UnityEngine;
using System.Collections;

using LitJson;

public class JsonExample : MonoBehaviour {
	
	public class ExampleSerializedClass {

		// Value Types

		public int myInt = 42;
		public string myString = "The quick brown fox jumped over the lazy dog.";
		public float myFloat = 3.14159f;
		public bool myBool = false;

		// Other supported value types:
		// uint, decimal, double, long,
		// ulong, byte, sbyte, short, ushort

		// Unity3D Types

		public Vector2 myVector2 = Vector2.one;
		public Vector3 myVector3 = Vector3.one * 3.14159f;
		public Vector4 myVector4 = Vector4.one * 6.28318f;
		public Quaternion myQuaternion = Quaternion.identity;

		public Color myColor = Color.red;
		public Color32 myColor32 = new Color32(85,127,255,255);

		public Bounds myBounds = new Bounds(Vector3.zero,Vector3.one);
		public Rect myRect = new Rect(10,10,25,25);
		public RectOffset myRectOffset = new RectOffset(5,10,15,20);

		public int[] myIntArray = new int[]{ 2, 4, 6, 8, 10 };

		[JsonIgnore]
		public Transform myIgnoredTransform; // This object will be ignored by the json engine.

		public ExampleSerializedClass(){ }

	}

	// Notice how some values have been changed.
	const string savedJsonString = @"{
		""myInt"" : 42,
		""myString"" : ""This value has changed!"",
		""myFloat""  : 6.28318,
		""myBool""   : true,
		""myVector2"" : {
			""x"" : 8.0,
			""y"" : 8.0
		},
		""myVector3"" : {
			""x"" : 3.1415901184082,
			""y"" : 3.1415901184082,
			""z"" : 3.1415901184082
		},
		""myVector4"" : {
			""x"" : 6.28318023681641,
			""y"" : 6.28318023681641,
			""z"" : 6.28318023681641,
			""w"" : 6.28318023681641
		},
		""myQuaternion"" : {
			""x"" : 1.0,
			""y"" : 1.0,
			""z"" : 1.0,
			""w"" : 1.0
		},
		""myColor""      : {
			""r"" : 0.0,
			""g"" : 0.0,
			""b"" : 0.0,
			""a"" : 0.0
		},
		""myColor32""    : {
			""r"" : 85,
			""g"" : 127,
			""b"" : 255,
			""a"" : 0
		},
		""myBounds""     : {
			""center"" : {
				""x"" : 0.0,
				""y"" : 0.0,
				""z"" : 0.0
			},
			""size""   : {
				""x"" : 1.0,
				""y"" : 1.0,
				""z"" : 1.0
			}
		},
		""myRect""       : {
			""x"" : 10.0,
			""y"" : 10.0,
			""width"" : 25.0,
			""height"" : 25.0
		},
		""myRectOffset"" : {
			""top"" : 15,
			""left"" : 5,
			""bottom"" : 20,
			""right""  : 10
		},
		""myIntArray""   : [
			   12,
			   14,
			   16,
			   18,
			   20
			   ]
	}";

	void Start () {

		ExampleSerializedClass serializedClass = new ExampleSerializedClass();

		JsonWriter writer = new JsonWriter();
		writer.PrettyPrint = true;

		JsonMapper.ToJson(serializedClass,writer);

		string json = writer.ToString();
		Debug.Log(json);

		// If you don't need a JsonWriter, use this.
		//string json = JsonMapper.ToJson(exampleClass);

		ExampleSerializedClass deserializedClass = JsonMapper.ToObject<ExampleSerializedClass>(savedJsonString);

		Debug.Log(deserializedClass.myString);

	}

}
