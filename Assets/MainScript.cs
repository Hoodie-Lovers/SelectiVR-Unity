using SFB;
using UnityEngine;
using UnityEngine.InputSystem;
using GLTFast;
using GLTFast.Logging;
using System.Threading;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;
using Button = UnityEngine.UI.Button;

public class AnimationControllerScript : MonoBehaviour
{
    private Keyboard keyboard;
    private bool CustomMode = false; //현재 커스텀 모드가 켜져있는지 확인하는 상태


    public Transform parentTransform;
    public Transform MainTransform;

    public GameObject Body;
    public GameObject Legs;
    public GameObject Arms;
    public GameObject Eyes;
    public GameObject Mouth;
    private int[] PartsNum = new int[5]; //Body,Legs,Arms,Eyes,Mouth
    private int[] PartsColor = new int[5];
    private GameObject loadedGlb; //Real Body


    public Canvas CustomModeUI;
    private GameObject SelectedParts;
    private bool selectPartBtn;

    public Vector3 rotationSpeed = new Vector3(0, 10f, 0);

    void Start()
    {
        keyboard = Keyboard.current;
        RefreshCustomMode();
        CustomModeUI.transform.Find("CustomBtn").GetComponent<Button>().onClick.AddListener(() => CustomOnOff());
        CustomModeUI.transform.Find("CustomUI").transform.Find("ImportBody").GetComponent<Button>().onClick.AddListener(() => OpenFileExplorer()); //파일 불러오기 예시
        CustomModeUI.transform.Find("CustomUI").transform.Find("LeftTurn").GetComponent<Button>().onClick.AddListener(() => rotationSpeed = new Vector3(0, 10f, 0));
        CustomModeUI.transform.Find("CustomUI").transform.Find("RightTurn").GetComponent<Button>().onClick.AddListener(() => rotationSpeed = new Vector3(0, -10f, 0));
        CustomModeUI.transform.Find("CustomUI").transform.Find("SelectParts").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").transform.Find("EyeParts").GetComponent<Button>().onClick.AddListener(() => Selecting(ref Eyes));
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").transform.Find("MouthParts").GetComponent<Button>().onClick.AddListener(() => Selecting(ref Mouth));
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").transform.Find("ArmParts").GetComponent<Button>().onClick.AddListener(() => Selecting(ref Arms));
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").transform.Find("LegParts").GetComponent<Button>().onClick.AddListener(() => Selecting(ref Legs));
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").transform.Find("BodyParts").GetComponent<Button>().onClick.AddListener(() => Selecting(ref Body));
        CustomModeUI.transform.Find("CustomUI").transform.Find("ColorChange").GetComponent<Button>().onClick.AddListener(() => CustomMaterial(ref SelectedParts));
        CustomModeUI.transform.Find("CustomUI").transform.Find("PartsChange").GetComponent<Button>().onClick.AddListener(() => PartChanging(ref SelectedParts));
    }


    void Update()
    {
        if (CustomMode)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        if (keyboard.spaceKey.wasPressedThisFrame) //모션 변화 함수 예시
        {
            ChangingMotion(Legs, "Walk");
            ChangingMotion(Arms, "Walk");
            ChangingMotion(Body, "YES");
        }

        if (keyboard.zKey.wasPressedThisFrame)//커스텀 함수 예시
        {
            if (CustomMode)
            {
                CustomParts(ref Legs, 0, 0, 0, -0.2f, -0.2f, -0.2f);
            }
        }

        if (keyboard.dKey.wasPressedThisFrame) //말하기 예시
        {
            ControlTalking(true);
        }
        if (keyboard.fKey.wasPressedThisFrame)
        {
            ControlTalking(false);
        }
    }

    public void ControlTalking(bool talk)
    {
        Mouth.GetComponent<MouthAnimator>().ControlTalking(talk);
    }

    public void Selecting(ref GameObject Object)
    {
        SelectedParts = Object;
        RefreshCustomMode();
    }

    public void SelectPartVisible()
    {
        if (selectPartBtn)
        {
            selectPartBtn = false;
            CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").gameObject.SetActive(false);
        }
        else
        {
            selectPartBtn = true;
            CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").gameObject.SetActive(true);
        }

    }


    public void ChangingMotion(GameObject Object, string type)
    {
        Animator animator = Object.GetComponent<Animator>();
        if (!HasParameter(animator, type, AnimatorControllerParameterType.Trigger))
        {
            Debug.Log($"'{type}' is not Exist in '{Object.name}'");
        }
        animator.SetTrigger(type);
    }
    private static bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }


    public void CustomOnOff()
    {
        if (!CustomMode)
        {
            CustomMode = true;
            selectPartBtn = false;
            CustomModeUI.transform.Find("CustomUI").transform.Find("PartsButton").gameObject.SetActive(false);
        }
        else
        {
            CustomMode = false;
            transform.rotation = Quaternion.identity;
        }
        RefreshCustomMode();
    }

    public void PartChanging(ref GameObject Object)
    {
        if (Object != null)
        {
            if (Object == Body)
            {
                PartsColor[0] = 0;

            }


            if (Object == Legs)
            {
                PartsColor[1] = 0;
                switch (PartsNum[1])
                {
                    case 0:
                        PartsNum[1] = 1;
                        ChangeParts(ref Legs, "LegParts2", new Vector3(0, -1, 0), new Vector3(0, -90, 0), new Vector3(0.3f, 0.3f, 0.3f));
                        Selecting(ref Legs);
                        break;
                    default:
                        PartsNum[1] = 0;
                        ChangeParts(ref Legs, "LegParts", new Vector3(0, -3, 0), new Vector3(0, 90, 0), new Vector3(1.0f, 1.0f, 1.0f));
                        Selecting(ref Legs);
                        break;
                }
            }

            if (Object == Arms)
            {
                PartsColor[2] = 0;
                switch (PartsNum[2])
                {
                    case 0:
                        PartsNum[2] = 1;
                        ChangeParts(ref Arms, "ArmParts2", new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(0.5f, 0.5f, 0.5f));
                        Selecting(ref Arms);
                        break;
                    default:
                        PartsNum[2] = 0;
                        ChangeParts(ref Arms, "ArmParts", new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0.3f, 0.3f, 0.3f));
                        Selecting(ref Arms);
                        break;
                }
            }

            if (Object == Eyes)
            {
                PartsColor[3] = 0;
                switch (PartsNum[3])
                {
                    case 0:
                        PartsNum[3] = 1;
                        ChangeParts(ref Eyes, "EyeParts2", new Vector3(0, 0, -1), new Vector3(0, 90, 0), new Vector3(1.0f, 1.0f, 1.0f));
                        Selecting(ref Eyes);
                        break;
                    default:
                        PartsNum[3] = 0;
                        ChangeParts(ref Eyes, "EyeParts", new Vector3(0, 0, -1), new Vector3(-90, 0, -90), new Vector3(40.0f, 40.0f, 40.0f));
                        Selecting(ref Eyes);
                        break;
                }
            }

            if (Object == Mouth)
            {
                PartsColor[4] = 0;
                switch (PartsNum[4])
                {
                    case 0:
                        PartsNum[4] = 1;
                        ChangeParts(ref SelectedParts, "Lips");
                        break;
                    default:
                        PartsNum[4] = 0;
                        ChangeParts(ref SelectedParts, "Mouth");
                        break;
                }
            }
        }
    }



    public void ChangeParts(
    ref GameObject oldObject,
    string newObject,
    Vector3 PrePosition = default(Vector3),
    Vector3 PreRotation = default(Vector3),
    Vector3 PreScale = default(Vector3))
    {
        if (CustomMode)
        {
            if (oldObject == Mouth)
            {
                Mouth.GetComponent<MouthAnimator>().ChangeMouth(newObject);
            }
            else
            {
                string ObjectPath = "Parts/" + newObject;
                string ControllerPath = "Controller/AC_" + newObject;
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(ControllerPath);
                if (oldObject != null)
                {
                    Destroy(oldObject);
                    Debug.Log("삭제완료");
                }

                GameObject _newObject = Resources.Load<GameObject>(ObjectPath);
                if (_newObject != null)
                {
                    if (oldObject == Eyes)
                    {
                        oldObject = Instantiate(_newObject, Body.transform);
                    }
                    else
                    {
                        oldObject = Instantiate(_newObject, MainTransform);
                    }
                    oldObject.transform.localPosition = PrePosition;
                    oldObject.transform.localRotation = Quaternion.Euler(PreRotation);
                    oldObject.transform.localScale = PreScale;
                    oldObject.AddComponent<Animator>();
                    Animator animator = oldObject.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.runtimeAnimatorController = controller;
                        Debug.Log("컨트롤러 연결");
                    }
                    Debug.Log("프리팹 생성");
                }
            }

        }
    }



    public void CustomParts(
        ref GameObject Object,
        float addLocationX = 0.5f,
        float addLocationY = 0.5f,
        float addLocationZ = 0.5f,
        float addScaleX = 0.5f,
        float addScaleY = 0.5f,
        float addScaleZ = 0.5f
        )
    {
        if (Object == null)
        {
            Debug.LogError("Object가 설정되어 있지 않습니다.");
            return;
        }
        else
        {
            SelectedParts = Object;
            RefreshCustomMode();

            if (CustomMode)
            {
                Object.transform.localPosition = new Vector3(
                    Object.transform.localPosition.x + addLocationX,
                    Object.transform.localPosition.y + addLocationY,
                    Object.transform.localPosition.z + addLocationZ);


                Object.transform.localScale = new Vector3(
                    Object.transform.localScale.x + addScaleX,
                    Object.transform.localScale.y + addScaleY,
                    Object.transform.localScale.z + addScaleZ);
            }
            else
            {
                Debug.Log("CustomMode is False State");
            }
        }

    }


    public void CustomMaterial(ref GameObject Object)
    {
        string color = "White";
        int numP = 0;

        if (Object == Body)
        {
            if (loadedGlb != null)
            {
                Object = loadedGlb;
                numP = 0;
            }
            else
            {
                Debug.Log("No body");
                return;
            }
        }
        if (Object == Legs) { numP = 1; }
        if (Object == Arms) { numP = 2; }
        if (Object == Eyes) { numP = 3; }
        if (Object == Mouth) { numP = 4; }

        switch (PartsColor[numP])
        {
            case 0:
                color = "Red";
                break;

            case 1:
                color = "Orange";
                break;

            case 2:
                color = "Yellow";
                break;
            case 3:
                color = "Green";
                break;
            case 4:
                color = "Blue";
                break;
            case 5:
                color = "Purple";
                break;
            case 6:
                color = "Brown";
                break;
            case 7:
                color = "Black";
                break;

            default:
                color = "White";
                PartsColor[numP] = -1;
                break;
        }
        PartsColor[numP] += 1;

        UnityEngine.Material loadedMat = Resources.Load<UnityEngine.Material>("Material/M_" + color);
        if (loadedMat != null)
        {
            //혹시 본체가 머터리얼가지고 있으면 본체도 바꾸기
            Renderer Orend = Object.GetComponent<Renderer>();
            if (Orend != null)
            {
                Orend.material = loadedMat;
            }

            //자식들 다 가져와서 머터리얼 바꾸기
            Renderer[] renderers = Object.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in renderers)
                rend.material = loadedMat;


            Debug.Log("머터리얼 변경완료");

        }
    }


    private void RefreshCustomMode()
    {
        CustomModeUI.transform.Find("CustomUI").gameObject.SetActive(CustomMode);
        if (SelectedParts != null)
        {
            CustomModeUI.transform.Find("CustomUI").transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = SelectedParts.name;
        }
        else
        {
            CustomModeUI.transform.Find("CustomUI").transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = "없음";
        }
    }

    public void OpenFileExplorer()
    {
        var extensions = new[] {
            new ExtensionFilter("GLB Files", "glb"),
            new ExtensionFilter("All Files", "*"),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select GLB File", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log("선택한 파일 경로: " + paths[0]);
            if (loadedGlb != null)
            {
                Destroy(loadedGlb);
                loadedGlb = null;
            }
            LoadGLB(paths[0]);
        }
        else
        {
            Debug.Log("파일 선택 취소됨 또는 경로 없음");
        }
    }

    private async void LoadGLB(string path)
    {
        if (parentTransform == null)
        {
            Debug.LogError("parentTransform이 설정되어 있지 않습니다.");
            return;
        }

        var logger = new ConsoleLogger();
        var gltf = new GltfImport(logger: logger);

        bool success = await gltf.Load(path);
        if (success)
        {
            Debug.Log("GLB 파일 로드 성공, 인스턴스화 시작!");

            int beforeChildCount = parentTransform.childCount;
            var instantiator = new GameObjectInstantiator(gltf, parentTransform);

            success = await gltf.InstantiateMainSceneAsync(instantiator, CancellationToken.None);

            if (success)
            {
                Debug.Log("GLB 인스턴스 생성 완료! parentTransform 하위에 부착 완료.");
                parentTransform.localPosition = new Vector3(0, 0, 0);
                parentTransform.localRotation = Quaternion.identity;
                parentTransform.localScale = Vector3.one;

                for (int i = beforeChildCount; i < parentTransform.childCount; i++)
                {
                    var newChild = parentTransform.GetChild(i).gameObject;

                    newChild.name = "MyLoadedGLB";
                    loadedGlb = newChild;
                }
            }
            else
            {
                Debug.LogError("GLB 인스턴스 생성 실패!");
            }
        }
        else
        {
            Debug.LogError("GLB 파일 로드 실패!");
        }
    }

    private async void DownloadAndLoadGLB(string url)
    {
        string localPath = Path.Combine(Application.persistentDataPath, "model.glb");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = new DownloadHandlerFile(localPath);
            var asyncOp = www.SendWebRequest();

            while (!asyncOp.isDone)
                await System.Threading.Tasks.Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GLB 다운로드 실패: " + www.error);
                return;
            }

            Debug.Log("GLB 다운로드 성공: " + localPath);
            LoadGLB(localPath);
        }
    }

}
