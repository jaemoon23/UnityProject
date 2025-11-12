using UnityEngine;
using NovelianMagicLibraryDefense.Managers;

/// <summary>
/// Simple test script for AudioManager functionality
/// Attach to any GameObject to test AudioManager features
/// </summary>
public class AudioManagerTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== AudioManager Test Started ===");
        TestAudioManager();
    }

    private void Update()
    {
        // Test keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[Test] Key 1: Testing PlayBGM");
            // AudioManager.Instance.PlayBGM("TestBGM");
            Debug.Log("[Test] PlayBGM would be called (need AudioClip in Addressables)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[Test] Key 2: Testing StopBGM");
            AudioManager.Instance.StopBGM();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("[Test] Key 3: Testing PlaySFX");
            // AudioManager.Instance.PlaySFX("TestSFX");
            Debug.Log("[Test] PlaySFX would be called (need AudioClip in Addressables)");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("[Test] Key Q: Increase Master Volume");
            float vol = AudioManager.Instance.GetMasterVolume();
            AudioManager.Instance.SetMasterVolume(Mathf.Clamp01(vol + 0.1f));
            Debug.Log($"[Test] Master Volume: {AudioManager.Instance.GetMasterVolume():F2}");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("[Test] Key A: Decrease Master Volume");
            float vol = AudioManager.Instance.GetMasterVolume();
            AudioManager.Instance.SetMasterVolume(Mathf.Clamp01(vol - 0.1f));
            Debug.Log($"[Test] Master Volume: {AudioManager.Instance.GetMasterVolume():F2}");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("[Test] Key S: Save Audio Settings");
            AudioManager.Instance.SaveAudioSettings();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("[Test] Key L: Load Audio Settings");
            AudioManager.Instance.LoadAudioSettings();
        }
    }

    private void TestAudioManager()
    {
        // Test 1: Check if AudioManager exists
        if (AudioManager.Instance != null)
        {
            Debug.Log("[Test] ✅ AudioManager Instance exists");
        }
        else
        {
            Debug.LogError("[Test] ❌ AudioManager Instance is NULL!");
            return;
        }

        // Test 2: Check volume control
        Debug.Log("[Test] Testing volume control...");

        AudioManager.Instance.SetMasterVolume(0.75f);
        Debug.Log($"[Test] Master Volume set to: {AudioManager.Instance.GetMasterVolume()}");

        AudioManager.Instance.SetBGMVolume(0.6f);
        Debug.Log($"[Test] BGM Volume set to: {AudioManager.Instance.GetBGMVolume()}");

        AudioManager.Instance.SetSFXVolume(0.9f);
        Debug.Log($"[Test] SFX Volume set to: {AudioManager.Instance.GetSFXVolume()}");

        // Test 3: Test settings persistence
        Debug.Log("[Test] Testing settings save/load...");
        AudioManager.Instance.SaveAudioSettings();
        Debug.Log("[Test] ✅ Audio settings saved");

        // Test 4: Stop BGM (safe to call even if nothing is playing)
        AudioManager.Instance.StopBGM();
        Debug.Log("[Test] ✅ StopBGM called successfully");

        Debug.Log("=== AudioManager Test Complete ===");
        Debug.Log("[Test Instructions]");
        Debug.Log("Press 1: Play BGM (needs Addressable AudioClip)");
        Debug.Log("Press 2: Stop BGM");
        Debug.Log("Press 3: Play SFX (needs Addressable AudioClip)");
        Debug.Log("Press Q: Increase Master Volume");
        Debug.Log("Press A: Decrease Master Volume");
        Debug.Log("Press S: Save Settings");
        Debug.Log("Press L: Load Settings");
    }
}
