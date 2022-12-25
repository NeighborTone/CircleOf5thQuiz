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
    public static T Random<T>(int min, int max)
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .OrderBy(c => mRandom.Next(min, max))
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
    }

    enum MajorKeySharp
    {
        Invalid = -1,
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
        Invalid = -1,
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
        Invalid = -1,
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
        Invalid = -1,
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
    [SerializeField] TMPro.TMP_InputField majInput = null;
    [SerializeField] TMPro.TMP_InputField minInput = null;
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
            case MajorKeyFlatto.C: minFlatto = MinorKeyFlatto.Am; break;
            case MajorKeyFlatto.F: minFlatto = MinorKeyFlatto.Dm; break;
            case MajorKeyFlatto.B_f: minFlatto = MinorKeyFlatto.Gm; break;
            case MajorKeyFlatto.E_f: minFlatto = MinorKeyFlatto.Cm; break;
            case MajorKeyFlatto.A_f: minFlatto = MinorKeyFlatto.Fm; break;
            case MajorKeyFlatto.D_f: minFlatto = MinorKeyFlatto.Bm_f; break;
            case MajorKeyFlatto.G_f: minFlatto = MinorKeyFlatto.Em_f; break;
            case MajorKeyFlatto.C_f: minFlatto = MinorKeyFlatto.Am_f; break;
            default: minFlatto = MinorKeyFlatto.Am; break;
        }
    }

    void RelativeKey(MinorKeyFlatto minFlatto, out MajorKeyFlatto majFlatto)
    {
        switch (minFlatto)
        {
            case MinorKeyFlatto.Am: majFlatto = MajorKeyFlatto.C; break;
            case MinorKeyFlatto.Dm: majFlatto = MajorKeyFlatto.F; break;
            case MinorKeyFlatto.Gm: majFlatto = MajorKeyFlatto.B_f; break;
            case MinorKeyFlatto.Cm: majFlatto = MajorKeyFlatto.E_f; break;
            case MinorKeyFlatto.Fm: majFlatto = MajorKeyFlatto.A_f; break;
            case MinorKeyFlatto.Bm_f: majFlatto = MajorKeyFlatto.D_f; break;
            case MinorKeyFlatto.Em_f: majFlatto = MajorKeyFlatto.G_f; break;
            case MinorKeyFlatto.Am_f: majFlatto = MajorKeyFlatto.C_f; break;
            default: majFlatto = MajorKeyFlatto.C; break;
        }
    }
    //

    void Start()
    {
        ResultTextShow(false);
        GenerateQuiz();
    }

    //決定ボタンから呼ばれる
    public void Anser()
    {
        if (currentAccidental == Accidental.Sharp)
        {
            if (anserMajSharp == quizMajSharp)
            {
                resultMajText.text = "正解！";
            }
            else
            {
                resultMajText.text = "不正解！";
            }

            if (anserMinSharp == quizMinSharp)
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
        if (currentAccidental == Accidental.Sharp)
        {
            sharpGroupObj.SetActive(true);
            flattoGroupObj.SetActive(false);
        }
        else
        {
            sharpGroupObj.SetActive(false);
            flattoGroupObj.SetActive(true);
        }
    }
    //調号の作成
    void GenerateKeySignature(List<GameObject> list, int signNum)
    {
        for (int i = 0; i < signNum; ++i)
        {
            list[i].SetActive(true);
        }
    }

    //クイズの作成処理。問題生成ボタンから呼ばれる
    void GenerateQuiz()
    {
        majInput.text = "";
        minInput.text = "";
        if (currentAccidental == Accidental.Sharp)
        {
            SwitchAccidental();
            foreach (var s in sharpList)
            {
                s.SetActive(false);
            }
            var quiz = EnumCommon.Random<MajorKeySharp>((int)MajorKeySharp.C, (int)MajorKeySharp.C_s + 1);
            //同じ問題が連続しないようにする
            if (quizMajSharp == quiz)
            {
                GenerateQuiz();
                return;
            }
            else
            {
                quizMajSharp = quiz;
            }
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
            var quiz = EnumCommon.Random<MajorKeyFlatto>((int)MajorKeyFlatto.C, (int)MajorKeyFlatto.C_f + 1);
            //同じ問題が連続しないようにする
            if (quizMajFlatto == quiz)
            {
                GenerateQuiz();
                return;
            }
            else
            {
                quizMajFlatto = quiz;
            }
            RelativeKey(quizMajFlatto, out quizMinFlatto);
            GenerateKeySignature(flattoList, (int)quizMajFlatto);
        }
        ResultTextShow(false);
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

    //以下回答処理
    //インプットフィールドが確定されたときに実行するメソッド
    public void EditEndMajorKey()
    {
        switch (majInput.text)
        {
            case "A":  anserMajSharp = MajorKeySharp.A;       anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "B":  anserMajSharp = MajorKeySharp.B;       anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "C":  anserMajSharp = MajorKeySharp.C;       anserMajFlatto = MajorKeyFlatto.C; break;
            case "D":  anserMajSharp = MajorKeySharp.D;       anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "E":  anserMajSharp = MajorKeySharp.E;       anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "F":  anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.F; break;
            case "G":  anserMajSharp = MajorKeySharp.G;       anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "A#": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "B#": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "C#": anserMajSharp = MajorKeySharp.C_s;     anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "D#": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "E#": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "F#": anserMajSharp = MajorKeySharp.F_s;     anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "G#": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "Ab": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.A_f; break;
            case "Bb": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.B_f; break;
            case "Cb": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.C_f; break;
            case "Db": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.D_f; break;
            case "Eb": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.E_f; break;
            case "Fb": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "Gb": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.G_f; break;
            case "A♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.A_f; break;
            case "B♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.B_f; break;
            case "C♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.C_f; break;
            case "D♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.D_f; break;
            case "E♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.E_f; break;
            case "F♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.Invalid; break;
            case "G♭": anserMajSharp = MajorKeySharp.Invalid; anserMajFlatto = MajorKeyFlatto.G_f; break;
        }
    }

    public void EditEndMinorKey()
    {
        switch (minInput.text)
        {
            case "A":  anserMinSharp = MinorKeySharp.Am;      anserMinFlatto = MinorKeyFlatto.Am; break;
            case "B":  anserMinSharp = MinorKeySharp.Bm;      anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "C":  anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Cm; break;
            case "D":  anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Dm; break;
            case "E":  anserMinSharp = MinorKeySharp.Em;      anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "F":  anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Fm; break;
            case "G":  anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Gm; break;
            case "A#": anserMinSharp = MinorKeySharp.Am_s;    anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "B#": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "C#": anserMinSharp = MinorKeySharp.Cm_s;    anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "D#": anserMinSharp = MinorKeySharp.Dm_s;    anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "E#": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "F#": anserMinSharp = MinorKeySharp.Fm_s;    anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "G#": anserMinSharp = MinorKeySharp.Gm_s;    anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "Ab": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Am_f; break;
            case "Bb": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Bm_f; break;
            case "Cb": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "Db": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "Eb": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Em_f; break;
            case "Fb": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "Gb": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "A♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Am_f; break;
            case "B♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Bm_f; break;
            case "C♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "D♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "E♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Em_f; break;
            case "F♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
            case "G♭": anserMinSharp = MinorKeySharp.Invalid; anserMinFlatto = MinorKeyFlatto.Invalid; break;
        }
    }
    //
}
