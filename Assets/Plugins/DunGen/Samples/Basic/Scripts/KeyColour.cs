using UnityEngine;

namespace DunGen.Demo
{
	public class KeyColour : MonoBehaviour, IKeyLock
	{
		[SerializeField]
		private int keyID;

		[SerializeField]
		private KeyManager keyManager;


		public void OnKeyAssigned(Key key, KeyManager manager)
		{
			keyID = key.ID;
			keyManager = manager;

			SetColour(key.Colour);
		}

		private void Start()
		{
			if (keyManager == null)
				return;

			var key = keyManager.GetKeyByID(keyID);
			SetColour(key.Colour);
		}

		private void SetColour(Color colour)
		{
			if (Application.isPlaying)
			{
				foreach (var r in GetComponentsInChildren<Renderer>())
					r.material.color = colour;
			}
		}
	}
}