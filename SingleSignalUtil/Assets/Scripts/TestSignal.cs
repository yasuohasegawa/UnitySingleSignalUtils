
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "SingleSignal/CreateSignal")]
public class TestSignal : SignalAsset {
	 public enum TestSignalID {
		StartAnimation,
		Test1,
		Test2,
		Test3,
		AnimationEnd
	 }
	 public TestSignalID SignalID;

	 public string[] GetSignalEnums() {
		 string[] typeNames = System.Enum.GetNames(typeof(TestSignalID));
		 return typeNames;
	 }

	 public TestSignalID GetSignalEnum(string enumVal){
		 TestSignalID e = (TestSignalID)System.Enum.Parse(typeof(TestSignalID), enumVal);
		 return e;
	 }

	 public void SetSignalEnum(TestSignalID enumVal){
		 SignalID = enumVal;
	 }

}
