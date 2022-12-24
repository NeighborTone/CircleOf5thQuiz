using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

//編集不可のパラメータをInspectorに表示する
//https://kazupon.org/unity-no-edit-param-view-inspector/
public class ReadOnlyAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(_position, _property, _label);
        EditorGUI.EndDisabledGroup();
    }
}
#endif
//

//列挙型の乱数を取得する
//https://baba-s.hatenablog.com/entry/2014/02/20/000000
/// <summary>
/// 列挙型に関する汎用クラス
/// </summary>
public static class EnumCommon
{
    private static readonly System.Random mRandom = new System.Random();  // 乱数

    /// <summary>
    /// 指定された列挙型の値をランダムに返します
    /// </summary>
    public static T Random<T>()
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .OrderBy(c => mRandom.Next())
            .FirstOrDefault();
    }
    
    /// <summary>
    /// 指定された列挙型の値の数返します
    /// </summary>
    public static int GetLength<T>()
    {
        return Enum.GetValues(typeof(T)).Length;
    }
}
//

public class CircleOf5th : MonoBehaviour
{
    //臨時記号#♭
    enum Accidental
    {
        Sharp,
        Flatto,
        NONE
    }

    enum MajorKeySharp
    {
        C = 0,
        G,
        D,
        A,
        E,
        B,
        F_s,
        C_s,
    }

    enum MinorKeySharp
    {
        Am = 0,
        Em,
        Bm,
        Fm_s,
        Cm_s,
        Gm_s,
        Dm_s,
        Am_s,
    }

    enum MajorKeyFlatto
    {
        C = 0,
        F,
        B_f,
        E_f,
        A_f,
        D_f,
        G_f,
        C_f,
    }

    enum MinorKeyFlatto
    {
        Am = 0,
        Dm,
        Gm,
        Cm,
        Fm,
        Bm_f,
        Em_f,
        Am_f,
    }

    [SerializeField, ReadOnly] Accidental currentAccidental = Accidental.Sharp;  //現在の問題が#系か♭系か
    //回答用
    [SerializeField, ReadOnly] MajorKeySharp anserMajSharp = default;
    [SerializeField, ReadOnly] MajorKeyFlatto anserMajFlatto = default;
    [SerializeField, ReadOnly] MinorKeySharp anserMinSharp = default;
    [SerializeField, ReadOnly] MinorKeyFlatto anserMinFlatto = default;
    //問題用
    [SerializeField, ReadOnly] MajorKeySharp quizMajSharp = default;
    [SerializeField, ReadOnly] MajorKeyFlatto quizMajFlatto = default;
    [SerializeField, ReadOnly] MinorKeySharp quizMinSharp = default;
    [SerializeField, ReadOnly] MinorKeyFlatto quizMinFlatto = default;

    [SerializeField] List<GameObject> sharpList = new(8);
    [SerializeField] List<GameObject> flattoList = new(8);
    [SerializeField] GameObject sharpGroupObj = default;
    [SerializeField] GameObject flattoGroupObj = default;
    [SerializeField] GameObject sharpDropGroupObj = default;
    [SerializeField] GameObject flattoDropGroupObj = default;
    [SerializeField] TMPro.TMP_Dropdown majSharpDrop = null;
    [SerializeField] TMPro.TMP_Dropdown minSharpDrop = null;
    [SerializeField] TMPro.TMP_Dropdown majFlattoDrop = null;
    [SerializeField] TMPro.TMP_Dropdown minFlattoDrop = null;
    [SerializeField] Toggle accidentalToggle = default;
    [SerializeField] TMPro.TextMeshProUGUI accidentalToggleText = default;
    [SerializeField] TMPro.TextMeshProUGUI resultMajText = default;
    [SerializeField] TMPro.TextMeshProUGUI resultMinText = default;

    void ResultTextShow(bool enable)
    {
        resultMajText.gameObject.SetActive(enable);
        resultMinText.gameObject.SetActive(enable);
    }

    //平行調を取得
    void RelativeKey(MajorKeySharp majSharp, out MinorKeySharp minSharp)
    {
        switch (majSharp)
        {
            case MajorKeySharp.C: minSharp = MinorKeySharp.Am; break;
            case MajorKeySharp.G: minSharp = MinorKeySharp.Em; break;
            case MajorKeySharp.D: minSharp = MinorKeySharp.Bm; break;
            case MajorKeySharp.A: minSharp = MinorKeySharp.Fm_s; break;
            case MajorKeySharp.E: minSharp = MinorKeySharp.Cm_s; break;
            case MajorKeySharp.B: minSharp = MinorKeySharp.Gm_s; break;
            case MajorKeySharp.F_s: minSharp = MinorKeySharp.Dm_s; break;
            case MajorKeySharp.C_s: minSharp = MinorKeySharp.Am_s; break;
            default: minSharp = MinorKeySharp.Am; break;
        }
    }

    void RelativeKey(MinorKeySharp minSharp, out MajorKeySharp majSharp)
    {
        switch (minSharp)
        {
            case MinorKeySharp.Am: majSharp = MajorKeySharp.C; break;
            case MinorKeySharp.Em: majSharp = MajorKeySharp.G; break;
            case MinorKeySharp.Bm: majSharp = MajorKeySharp.D; break;
            case MinorKeySharp.Fm_s: majSharp = MajorKeySharp.A; break;
            case MinorKeySharp.Cm_s: majSharp = MajorKeySharp.E; break;
            case MinorKeySharp.Gm_s: majSharp = MajorKeySharp.B; break;
            case MinorKeySharp.Dm_s: majSharp = MajorKeySharp.F_s; break;
            case MinorKeySharp.Am_s: majSharp = MajorKeySharp.C_s; break;
            default: majSharp = MajorKeySharp.C; break;
        }
    }

    void RelativeKey(MajorKeyFlatto majFlatto, out MinorKeyFlatto minFlatto)
    {
        switch (majFlatto)
        {
            case MajorKeyFlatto.C:    minFlatto = MinorKeyFlatto.Am; break;
            case MajorKeyFlatto.F:    minFlatto = MinorKeyFlatto.Dm; break;
            case MajorKeyFlatto.B_f:  minFlatto = MinorKeyFlatto.Gm; break;
            case MajorKeyFlatto.E_f:  minFlatto = MinorKeyFlatto.Cm; break;
            case MajorKeyFlatto.A_f:  minFlatto = MinorKeyFlatto.Fm; break;
            case MajorKeyFlatto.D_f:  minFlatto = MinorKeyFlatto.Bm_f; break;
            case MajorKeyFlatto.G_f:  minFlatto = MinorKeyFlatto.Em_f; break;
            case MajorKeyFlatto.C_f:  minFlatto = MinorKeyFlatto.Am_f; break;
            default: minFlatto = MinorKeyFlatto.Am; break;
        }
    }

    void RelativeKey(MinorKeyFlatto minFlatto, out MajorKeyFlatto majFlatto)
    {  
        switch (minFlatto)  
        {  
            case MinorKeyFlatto.Am:    majFlatto = MajorKeyFlatto.C;   break;
            case MinorKeyFlatto.Dm:    majFlatto = MajorKeyFlatto.F;   break;
            case MinorKeyFlatto.Gm:    majFlatto = MajorKeyFlatto.B_f; break;
            case MinorKeyFlatto.Cm:    majFlatto = MajorKeyFlatto.E_f; break;
            case MinorKeyFlatto.Fm:    majFlatto = MajorKeyFlatto.A_f; break;
            case MinorKeyFlatto.Bm_f:  majFlatto = MajorKeyFlatto.D_f; break;
            case MinorKeyFlatto.Em_f:  majFlatto = MajorKeyFlatto.G_f; break;
            case MinorKeyFlatto.Am_f:  majFlatto = MajorKeyFlatto.C_f; break;
            default: majFlatto = MajorKeyFlatto.C; break;
        }
    }
    //

    void Start()
    {
        ResultTextShow(false);
        GenerateQuizCallback();
    }

    //決定ボタンから呼ばれる
    public void Anser()
    {
        if (currentAccidental == Accidental.Sharp)
        {
            if(anserMajSharp == quizMajSharp)
            {
                resultMajText.text = "正解！";
            }
            else
            {
                resultMajText.text = "不正解！";
            }

            if(anserMinSharp == quizMinSharp)
            {
                resultMinText.text = "正解！";
            }
            else
            {
                resultMinText.text = "不正解！";
            }
        }
        else
        {
            if (anserMajFlatto == quizMajFlatto)
            {
                resultMajText.text = "正解！";
            }
            else
            {
                resultMajText.text = "不正解！";
            }

            if (anserMinFlatto == quizMinFlatto)
            {
                resultMinText.text = "正解！";
            }
            else
            {
                resultMinText.text = "不正解！";
            }
        }
        ResultTextShow(true);
    }

    //#系か♭系に応じて表示するオブジェクトを切り替える処理
    void SwitchAccidental()
    {
        if(currentAccidental == Accidental.Sharp)
        {
            sharpGroupObj.SetActive(true);
            sharpDropGroupObj.SetActive(true);
            flattoGroupObj.SetActive(false);
            flattoDropGroupObj.SetActive(false);            
        }
        else
        {
            sharpGroupObj.SetActive(false);
            sharpDropGroupObj.SetActive(false);
            flattoGroupObj.SetActive(true);
            flattoDropGroupObj.SetActive(true);              
        }
    }
    //調号の作成
    void GenerateKeySignature(List<GameObject> list, int signNum)
    {
        for(int i = 0; i < signNum; ++i)
        {
            list[i].SetActive(true);
        }
    }

    //クイズの作成処理
    void GenerateQuiz()
    {
        if (currentAccidental == Accidental.Sharp)
        {
            SwitchAccidental();
            foreach(var s in sharpList)
            {
                s.SetActive(false);
            }
            quizMajSharp = EnumCommon.Random<MajorKeySharp>();
            RelativeKey(quizMajSharp, out quizMinSharp);
            GenerateKeySignature(sharpList, (int)quizMajSharp);
        }
        else
        {
            SwitchAccidental();
            foreach (var f in flattoList)
            {
                f.SetActive(false);
            }
            quizMajFlatto = EnumCommon.Random<MajorKeyFlatto>();
            RelativeKey(quizMajFlatto, out quizMinFlatto);
            GenerateKeySignature(flattoList, (int)quizMajFlatto);
        }
    }

    //問題生成ボタンから呼ばれる
    public void GenerateQuizCallback()
    {
        ResultTextShow(false);
        SwitchAccidental();
    }

    //問題を#系か♭系か決めるトグルから呼ばれる
    public void ChangeAccidental()
    {
        if (accidentalToggle.isOn)
        {
            currentAccidental = Accidental.Sharp;
            accidentalToggleText.text = "+";
        }
        else
        {
            currentAccidental = Accidental.Flatto;
            accidentalToggleText.text = "-";
        }
    }

    // 以下ドロップダウンのオプションが変更されたときに実行するメソッド
    public void ChangeMajorKey()
    {
        if (currentAccidental == Accidental.Sharp)
        {
            switch (majSharpDrop.value)
            {
                case 0: anserMajSharp = MajorKeySharp.C; break;
                case 1: anserMajSharp = MajorKeySharp.G; break;
                case 2: anserMajSharp = MajorKeySharp.D; break;
                case 3: anserMajSharp = MajorKeySharp.A; break;
                case 4: anserMajSharp = MajorKeySharp.E; break;
                case 5: anserMajSharp = MajorKeySharp.B; break;
                case 6: anserMajSharp = MajorKeySharp.F_s; break;
                case 7: anserMajSharp = MajorKeySharp.C_s; break;
            }
        }
        else
        {
            switch (majFlattoDrop.value)
            {
                case 0: anserMajFlatto = MajorKeyFlatto.C; break;
                case 1: anserMajFlatto = MajorKeyFlatto.F; break;
                case 2: anserMajFlatto = MajorKeyFlatto.B_f; break;
                case 3: anserMajFlatto = MajorKeyFlatto.E_f; break;
                case 4: anserMajFlatto = MajorKeyFlatto.A_f; break;
                case 5: anserMajFlatto = MajorKeyFlatto.D_f; break;
                case 6: anserMajFlatto = MajorKeyFlatto.G_f; break;
                case 7: anserMajFlatto = MajorKeyFlatto.C_f; break;
            }
        }
    }

    public void ChangeMinorKey()
    {
        if (currentAccidental == Accidental.Sharp)
        {
            switch (minSharpDrop.value)
            {
                case 0: anserMinSharp = MinorKeySharp.Am; break;
                case 1: anserMinSharp = MinorKeySharp.Em; break;
                case 2: anserMinSharp = MinorKeySharp.Bm; break;
                case 3: anserMinSharp = MinorKeySharp.Fm_s; break;
                case 4: anserMinSharp = MinorKeySharp.Cm_s; break;
                case 5: anserMinSharp = MinorKeySharp.Gm_s; break;
                case 6: anserMinSharp = MinorKeySharp.Dm_s; break;
                case 7: anserMinSharp = MinorKeySharp.Am_s; break;
            }
        }
        else
        {
            switch (minFlattoDrop.value)
            {
                case 0: anserMinFlatto = MinorKeyFlatto.Am; break;
                case 1: anserMinFlatto = MinorKeyFlatto.Dm; break;
                case 2: anserMinFlatto = MinorKeyFlatto.Gm; break;
                case 3: anserMinFlatto = MinorKeyFlatto.Cm; break;
                case 4: anserMinFlatto = MinorKeyFlatto.Fm; break;
                case 5: anserMinFlatto = MinorKeyFlatto.Bm_f; break;
                case 6: anserMinFlatto = MinorKeyFlatto.Em_f; break;
                case 7: anserMinFlatto = MinorKeyFlatto.Am_f; break;
            }
        }
    }
    // 

}
