using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletRewardUI : MonoBehaviour
{
    private GameObject canvasObject;
    private RectTransform panel;
    private Text titleText;
    private Font font;
    private readonly List<GameObject> generatedButtons = new List<GameObject>();
    private Action<BulletDataSC.BulletInfo> onResolved;
    private bool resolved;

    public void Show(
        int clearedStage,
        IReadOnlyList<BulletDataSC.BulletInfo> choices,
        Action<BulletDataSC.BulletInfo> callback)
    {
        EnsureUi();
        ClearButtons();
        resolved = false;
        onResolved = callback;
        titleText.text = $"STAGE {clearedStage} CLEAR\n총알을 선택하세요";

        int count = choices != null ? choices.Count : 0;
        for (int i = 0; i < count; i++)
        {
            BulletDataSC.BulletInfo bullet = choices[i];
            float y = 95f - i * 90f;
            CreateChoiceButton(bullet, y);
        }

        CreateSkipButton(-190f);
        canvasObject.SetActive(true);
    }

    public void Hide()
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private void Resolve(BulletDataSC.BulletInfo selectedBullet)
    {
        if (resolved)
        {
            return;
        }

        resolved = true;
        Hide();
        Action<BulletDataSC.BulletInfo> callback = onResolved;
        onResolved = null;
        callback?.Invoke(selectedBullet);
    }

    private void EnsureUi()
    {
        if (canvasObject != null)
        {
            return;
        }

        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        canvasObject = new GameObject(
            "Bullet Reward Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject panelObject = CreateUiObject("Reward Panel", canvasObject.transform);
        panel = panelObject.AddComponent<Image>().rectTransform;
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(720f, 560f);
        panel.anchoredPosition = Vector2.zero;
        panelObject.GetComponent<Image>().color = new Color(0.06f, 0.07f, 0.1f, 0.96f);

        titleText = CreateText("Title", panel, 34, TextAnchor.MiddleCenter);
        RectTransform titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(650f, 110f);
        titleRect.anchoredPosition = new Vector2(0f, -65f);

        canvasObject.SetActive(false);
    }

    private void CreateChoiceButton(BulletDataSC.BulletInfo bullet, float y)
    {
        GameObject buttonObject = CreateButtonObject(
            $"{bullet.BulletName}\n{bullet.Rarity}  |  Damage {bullet.BulletDamage:0.##}",
            y,
            GetRarityColor(bullet.Rarity));
        buttonObject.GetComponent<Button>().onClick.AddListener(() => Resolve(bullet));
        generatedButtons.Add(buttonObject);
    }

    private void CreateSkipButton(float y)
    {
        GameObject buttonObject = CreateButtonObject(
            "선택하지 않기",
            y,
            new Color(0.25f, 0.25f, 0.28f, 1f));
        buttonObject.GetComponent<Button>().onClick.AddListener(() => Resolve(null));
        generatedButtons.Add(buttonObject);
    }

    private GameObject CreateButtonObject(string label, float y, Color color)
    {
        GameObject buttonObject = CreateUiObject(label, panel);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(570f, 72f);
        rect.anchoredPosition = new Vector2(0f, y);

        Text text = CreateText("Label", rect, 25, TextAnchor.MiddleCenter);
        text.color = Color.white;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = new Vector2(12f, 4f);
        text.rectTransform.offsetMax = new Vector2(-12f, -4f);
        text.text = label;
        return buttonObject;
    }

    private Text CreateText(
        string objectName,
        Transform parent,
        int fontSize,
        TextAnchor alignment)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        return result;
    }

    private void ClearButtons()
    {
        foreach (GameObject button in generatedButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }

        generatedButtons.Clear();
    }

    private static Color GetRarityColor(BulletRarity rarity)
    {
        return rarity switch
        {
            BulletRarity.Rare => new Color(0.1f, 0.35f, 0.75f, 1f),
            BulletRarity.Epic => new Color(0.5f, 0.18f, 0.7f, 1f),
            _ => new Color(0.3f, 0.32f, 0.36f, 1f)
        };
    }
}
