using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class SubtitleTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        string currentText = "";
        float currenAlpha = 0f;
        if (!text) { return; }

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWieght = playable.GetInputWeight(i);
            if (inputWieght > 0f)
            {
                ScriptPlayable<SubtitleBehaviour> inputPlayeable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour input = inputPlayeable.GetBehaviour();
                currentText = input.subtitle_text;
                currenAlpha = inputWieght;
            }
        }
        text.text = currentText;
        text.color = new Color(1, 1, 1, currenAlpha);

    }
}
