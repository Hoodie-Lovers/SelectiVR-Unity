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
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class AnimationControllerScript : MonoBehaviour
{
    private Keyboard keyboard;
    private bool CustomMode = true; //현재 커스텀 모드가 켜져있는지 확인하는 상태


    public Transform parentTransform;
    public Transform MainTransform;

    public GameObject Body;
    public GameObject Legs;
    public GameObject Arms;
    public GameObject Eyes;
    public GameObject Mouth;

    public Canvas CustomModeUI;
    private GameObject SelectedParts;
    private bool selectPartBtn;

    public Vector3 rotationSpeed = new Vector3(0, 10f, 0);

    void Start()
    {
        keyboard = Keyboard.current;
        RefreshCustomMode();
        CustomModeUI.transform.Find("ImportBody").GetComponent<Button>().onClick.AddListener(()=> OpenFileExplorer()); //파일 불러오기 예시
        CustomModeUI.transform.Find("LeftTurn").GetComponent<Button>().onClick.AddListener(() => rotationSpeed = new Vector3(0, 10f, 0));
        CustomModeUI.transform.Find("RightTurn").GetComponent<Button>().onClick.AddListener(() => rotationSpeed = new Vector3(0, -10f, 0));
        CustomModeUI.transform.Find("SelectParts").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
        CustomModeUI.transform.Find("Body").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
        CustomModeUI.transform.Find("SelectParts").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
        CustomModeUI.transform.Find("SelectParts").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
        CustomModeUI.transform.Find("SelectParts").GetComponent<Button>().onClick.AddListener(() => SelectPartVisible());
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

        if (keyboard.cKey.wasPressedThisFrame) //커스텀 모드를 키고 끄는 예시
        {
            if (!CustomMode)
            {
                CustomMode = true;
                selectPartBtn = false;
                CustomModeUI.transform.Find("PartsButton").gameObject.SetActive(false);
            }
            else
            {
                CustomMode = false;
                transform.rotation = Quaternion.identity;
            }
            RefreshCustomMode();
        }

        if (keyboard.xKey.wasPressedThisFrame)//커스텀 함수 예시
        {
            if (CustomMode)
            {
                CustomMaterial(ref Arms, "Red");
            }
        }

        if (keyboard.zKey.wasPressedThisFrame)//커스텀 함수 예시
        {
            if (CustomMode)
            {
                CustomParts(ref Legs, 0, 0, 0, -0.2f, -0.2f, -0.2f);
            }
        }

        if (keyboard.qKey.wasPressedThisFrame) //파츠 변화 함수 예시
        {
            ChangeParts(ref Arms, "ArmParts2", new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(0.5f, 0.5f, 0.5f));

            ChangeParts(ref Legs, "LegParts2", new Vector3(0, -1, 0), new Vector3(0, -90, 0), new Vector3(0.3f, 0.3f, 0.3f));
        }


        if (keyboard.dKey.wasPressedThisFrame) //말하기 예시
        {
            ControlTalking(true);
        }
        if (keyboard.fKey.wasPressedThisFrame)
        {
            ControlTalking(false);
        }

        if (keyboard.sKey.wasPressedThisFrame)
        {
            Selecting(ref Mouth);
            ChangeParts(ref SelectedParts, "Lips");
        }
    }

    public void ControlTalking(bool talk)
    {
        Mouth.GetComponent<MouthAnimator>().ControlTalking(talk);
    }

    public void Selecting(ref GameObject Object)
    {
        SelectedParts = Object;
    }

    public void SelectPartVisible()
    {
        if (selectPartBtn)
        {
            selectPartBtn = false;
            CustomModeUI.transform.Find("PartsButton").gameObject.SetActive(false);
        }
        else
        {
            selectPartBtn = true;
            CustomModeUI.transform.Find("PartsButton").gameObject.SetActive(true);
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
                    oldObject = Instantiate(_newObject, MainTransform);
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


    public void CustomMaterial(ref GameObject Object, String color)
    {
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
        CustomModeUI.gameObject.SetActive(CustomMode);
        if (SelectedParts != null)
        {
            CustomModeUI.transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = SelectedParts.name;
        }
        else
        {
            CustomModeUI.transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = "없음";
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

            var instantiator = new GameObjectInstantiator(gltf, parentTransform);

            success = await gltf.InstantiateMainSceneAsync(instantiator, CancellationToken.None);

            if (success)
            {
                Debug.Log("GLB 인스턴스 생성 완료! parentTransform 하위에 부착 완료.");
                parentTransform.localPosition = new Vector3(0, 0, 0);
                parentTransform.localRotation = Quaternion.identity;
                parentTransform.localScale = Vector3.one;
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
