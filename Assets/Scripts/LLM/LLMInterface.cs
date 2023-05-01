using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.Events;
using System.Threading.Tasks;

public class LLMInterface : MonoBehaviour
{
    public UnityEvent<string> onLLMResponse;

    public virtual void Init()
    {
    }

    public virtual async Task SendRequest(string request) {
    }
    public virtual async Task ResponseHandle(string response) {
    }
}
