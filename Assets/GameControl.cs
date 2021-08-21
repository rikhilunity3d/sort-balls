using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

//First Video
//https://www.youtube.com/watch?v=1Ji6APm0Lyg


public class GameControl : MonoBehaviour
{
    [SerializeField]
    private GameObject testTube1, testTube2, testTube3;
    [SerializeField]
    private GameObject EmptyTestTube1, EmptyTestTube2;

    [SerializeField]
    private List<balls> ballsObjects1, ballsObjects2, ballsObjects3;

    public List<GameObject> fillObjectsLocation1, fillObjectsLocation2, fillObjectsLocation3;

    public List<GameObject> emptyObjectsLocation1, emptyObjectsLocation2;

    [SerializeField]
    Transform[] wayPoints;
    int wayPointIndex = 0;

    private Stack<balls> stack1, stack2, stack3, stack4, stack5;
    public PathType path = PathType.CatmullRom;
    public Vector3[] pathVal = new Vector3[3];
    balls ball, tempBall;
    bool isBallOut;

    int touchTwice = 0;
    //int secondTouchedTwice = 1;
    int fTop = 3;
    int sTop = 3;
    int tTop = 3;
    int foTop = -1;
    int fiTop = -1;

    int fCount = 0;
    int sCount = 0;
    int tCount = 0;
    int foCount = 0;
    int fiCount = 0;

    int done=0;

 
    string testTube1MainColor = "";
    string testTube2MainColor = "";
    string testTube3MainColor = "";
    string testTube4MainColor = "";
    string testTube5MainColor = "";

    // Start is called before the first frame update
    void Start()
    {
        TestTubeTouch1.ButtonPressed += TestTubeTouch1ButtonPressed;
        TestTubeTouch2.ButtonPressed += TestTubeTouch2ButtonPressed;
        TestTubeTouch3.ButtonPressed += TestTubeTouch3ButtonPressed;
        EmptyTestTubeTouch1.ButtonPressed += EmptyTestTubeTouch1ButtonPressed;
        EmptyTestTubeTouch2.ButtonPressed += EmptyTestTubeTouch2ButtonPressed;

        isBallOut = false;
        touchTwice = 0;

        //Define your stack
        stack1 = new Stack<balls>(fillObjectsLocation1.Count);
        stack2 = new Stack<balls>(fillObjectsLocation2.Count);
        stack3 = new Stack<balls>(fillObjectsLocation3.Count);
        stack4 = new Stack<balls>(emptyObjectsLocation1.Count);
        stack5 = new Stack<balls>(emptyObjectsLocation2.Count);

        ball = null;

        // Prep our data
        foreach (var item in ballsObjects1)
        {
            stack1.Push(item);
        }
        foreach (var item in ballsObjects2)
        {
            stack2.Push(item);
        }
        foreach (var item in ballsObjects3)
        {
            stack3.Push(item);
        }
        showInformationInStack(stack1);
        showInformationInStack(stack2);
        showInformationInStack(stack3);
    }

    void Update()
    {
        if(done == 3)
        {
            Debug.Log("Congratulations You Win ");
        }
    }

    void showInformationInStack(Stack<balls> stack)
    {
        foreach (var stackItem in stack)
        {
            Debug.Log("StackItem = " + stackItem.index);
            Debug.Log("StackItem = " + stackItem.tag);
            Debug.Log("StackItem = " + stackItem.ballObject);
        }
    }

  


    private void TestTubeTouch1ButtonPressed()
    {
        Debug.Log("Touched TestTube One");
        if (fTop == -1)
        {
            testTube1MainColor = "";
            print("testTube1MainColor = " + testTube1MainColor);

            fCount = 0;
            print("fCount " + fCount);
        }

        if (touchTwice == 0)
        {
            //if touchTwice == 0 then
            //pop logic
            print("touchTwice =" + touchTwice);

            if (fTop > -1 && isBallOut == false)
            {
                print("Now Pop");
                ball = stack1.Pop();
                ball.ballObject.transform.DOMove(pathVal[0], 0.1f);

                isBallOut = true;
                touchTwice++;
                fTop--;

                fCount = 0;
                print("fCount " + fCount);

            }
            else
            {
                isBallOut = false;
                ball = null;
            }
        }
        else
        {
            //if touchTwice == 1 then
            // push logic
            print("touchTwice =" + touchTwice);
            if (isBallOut == true && fTop < 3)
            {
                if (testTube1MainColor == "")
                {
                    testTube1MainColor = ball.tag;
                }
                if (testTube1MainColor == ball.tag)
                {
                    print("Now Push");
                    fTop++;
                    stack1.Push(ball);
                    ball.ballObject.transform.DOMove(fillObjectsLocation1[fTop].gameObject.transform.position, 0.1f);

                    isBallOut = false;
                    ball = null;

                    print("testTube1MainColor = " + testTube1MainColor);

                    touchTwice--;

                    changeMainColorOfTestTube();
                }
            }
            else
            {
                Debug.Log("Stack1 is full");
            }
        }
        if (stack1.Count == 4)
        {
            foreach (var item in stack1)
            {
                if (item.tag == testTube1MainColor)
                {
                    fCount = 1;

                }
                else
                {
                    fCount = 0;

                    break;
                }
            }

            print("fCount " + fCount);

        }
        else
        {
            fCount = 0;
            print("fCount " + fCount);

        }

        if(fCount == 1)
        {
            done++;
            TestTubeTouch1.ButtonPressed -= TestTubeTouch1ButtonPressed;
        }
       
        print("Testtube1 done " + done);
    }

    private void TestTubeTouch2ButtonPressed()
    {
        if (sTop == -1)
        {
            testTube2MainColor = "";
            print("testTube2MainColor = " + testTube2MainColor);

            sCount = 0;
            print("sCount " + sCount);
        }
        Debug.Log("Touched TestTube two");
        if (touchTwice == 0)
        {
            //if touchTwice == 0 then
            //pop logic
            print("touchTwice =" + touchTwice);
            if (sTop > -1 && isBallOut == false)
            {
                print("Now Pop");
                ball = stack2.Pop();
                ball.ballObject.transform.DOMove(pathVal[0], 0.1f);
                sTop--;

                isBallOut = true;
                touchTwice++;

                sCount = 0;
                print("sCount " + sCount);

            }
            else
            {
                isBallOut = false;
                ball = null;
            }
        }
        else
        {
            //if touchTwice == 1 then
            // push logic
            print("touchTwice =" + touchTwice);
            if (isBallOut == true && sTop < 3)
            {
                if (testTube2MainColor == "")
                {
                    testTube2MainColor = ball.tag;
                }
                if (testTube2MainColor == ball.tag)
                {
                    print("Now Push");
                    sTop++;
                    stack2.Push(ball);
                    ball.ballObject.transform.DOMove(fillObjectsLocation2[sTop].gameObject.transform.position, 0.1f);

                    isBallOut = false;
                    ball = null;

                    print("testTube2MainColor = " + testTube2MainColor);

                    touchTwice--;

                    changeMainColorOfTestTube();
                }
            }
            else
            {
                Debug.Log("Stack2 is full");
            }
        }
        if (stack2.Count == 4)
        {
            foreach (var item in stack2)
            {
                if (item.tag == testTube2MainColor)
                {
                    sCount = 1;

                }
                else
                {
                    sCount = 0;

                    break;
                }
            }

            print("sCount " + sCount);

        }
        else
        {
            sCount = 0;
            print("sCount " + sCount);

        }

        if (sCount == 1)
        {
            done++;
            TestTubeTouch2.ButtonPressed -= TestTubeTouch2ButtonPressed;

        }
        print("Testtube2 done " + done);

    }

    private void TestTubeTouch3ButtonPressed()
    {
        if (tTop == -1)
        {
            testTube3MainColor = "";
            print("testTube3MainColor = " + testTube3MainColor);

            tCount = 0;
            print("tCount " + tCount);
        }
        Debug.Log("Touched TestTube three");
        if (touchTwice == 0)
        {
            //if touchTwice == 0 then
            //pop logic
            print("touchTwice =" + touchTwice);

            if (tTop > -1 && isBallOut == false)
            {
                print("Now Pop");
                ball = stack3.Pop();
                ball.ballObject.transform.DOMove(pathVal[0], 0.1f);
                tTop--;

                isBallOut = true;
                touchTwice++;

                tCount = 0;
                print("tCount " + tCount);
            }
            else
            {
                isBallOut = false;
                ball = null;
            }
        }
        else
        {
            //if touchTwice == 1 then
            // push logic
            print("touchTwice =" + touchTwice);

            if (isBallOut == true && tTop < 3)
            {
                if (testTube3MainColor == "")
                {
                    testTube3MainColor = ball.tag;
                }
                if (testTube3MainColor == ball.tag)
                {
                    print("Now Push");
                    tTop++;
                    stack3.Push(ball);
                    ball.ballObject.transform.DOMove(fillObjectsLocation3[tTop].gameObject.transform.position, 0.1f);

                    isBallOut = false;
                    ball = null;

                    print("testTube3MainColor = " + testTube3MainColor);

                    touchTwice--;

                    changeMainColorOfTestTube();
                }
            }
            else
            {
                Debug.Log("Stack3 is full");
            }
        }
        if (stack3.Count == 4)
        {
            foreach (var item in stack3)
            {
                if (item.tag == testTube3MainColor)
                {
                    tCount = 1;

                }
                else
                {
                    tCount = 0;

                    break;
                }
            }
            print("tCount " + tCount);
        }
        else
        {
            tCount = 0;
            print("tCount " + tCount);
        }

        if (tCount == 1)
        {
            done++;
            TestTubeTouch3.ButtonPressed -= TestTubeTouch3ButtonPressed;

        }
       
        print("Testtube3 done " + done);

    }

    // Testube Fourth but Empty Testube One
    private void EmptyTestTubeTouch1ButtonPressed()
    {
        if (foTop == -1)
        {
            testTube4MainColor = "";
            print("testTube4MainColor = " + testTube4MainColor);

            foCount = 0;
            print("foCount " + foCount);
        }
        
        Debug.Log("Touched TestTube Four");
        if (touchTwice == 0)
        {
            //if secondTouchedTwice == 0 then
            //pop logic
            print("touchTwice =" + touchTwice);

            if (foTop > -1 && isBallOut == false)
            {

                print("Now Pop");
                ball = stack4.Pop();
                ball.ballObject.transform.DOMove(pathVal[0], 0.1f);
                foTop--;

                isBallOut = true;
                touchTwice++;

                foCount = 0;
                print("foCount " + foCount);
            }
            else
            {
                isBallOut = false;
                ball = null;
            }
        }
        else
        {
            //if secondTouchedTwice == 1 then
            // push logic
            print("touchTwice =" + touchTwice);

            if (isBallOut == true && foTop < emptyObjectsLocation1.Count - 1)
            {
                if (testTube4MainColor == "")

                {
                    testTube4MainColor = ball.tag;
                }

                if (testTube4MainColor == ball.tag)
                {
                    print("Now Push");
                    foTop++;
                    stack4.Push(ball);
                    ball.ballObject.transform.DOMove(emptyObjectsLocation1[foTop].gameObject.transform.position, 0.1f);

                    isBallOut = false;
                    ball = null;

                    print("testTube4MainColor = " + testTube4MainColor);

                    touchTwice--;

                    changeMainColorOfTestTube();
                }
            }
            else
            {
                Debug.Log("Stack4 is full");
            }
        }
        if (stack4.Count == 4)
        {
            foreach (var item in stack4)
            {
                if (item.tag == testTube4MainColor)
                {
                    foCount = 1;

                }
                else
                {
                    foCount = 0;

                    break;
                }
            }

            print("foCount " + foCount);

        }
        else
        {
            foCount = 0;
            print("foCount " + foCount);

        }

        if (foCount == 1)
        {
            done++;
            EmptyTestTubeTouch1.ButtonPressed -= EmptyTestTubeTouch1ButtonPressed;
        }
       
        print("Testtube4 done " + done);
    }

    // Testube Fifth but Empty Testube two
    private void EmptyTestTubeTouch2ButtonPressed()
    {
        if (fiTop == -1)
        {
            testTube5MainColor = "";
            print("testTube5MainColor = " + testTube5MainColor);

            fiCount = 0;
            print("fiCount " + fiCount);
        }

        Debug.Log("Touched TestTube Five");
        if (touchTwice == 0)
        {
            //if secondTouchedTwice == 0 then
            //pop logic
            print("touchTwice =" + touchTwice);

            if (fiTop > -1 && isBallOut == false)
            {
                print("Now Pop");
                ball = stack5.Pop();
                ball.ballObject.transform.DOMove(pathVal[0], 0.1f);
                fiTop--;

                isBallOut = true;
                touchTwice++;

                fiCount = 0;
                print("fiCount " + fiCount);
            }
            else
            {
                isBallOut = false;
                ball = null;
            }
        }
        else
        {
            //if secondTouchedTwice == 1 then
            // push logic
            print("touchTwice =" + touchTwice);

            if (isBallOut == true && fiTop < emptyObjectsLocation2.Count - 1)
            {
                if (testTube5MainColor == "")
                {
                    testTube5MainColor = ball.tag;
                }

                if (testTube5MainColor == ball.tag)
                {
                    print("Now Push");
                    fiTop++;
                    stack5.Push(ball);
                    ball.ballObject.transform.DOMove(emptyObjectsLocation2[fiTop].gameObject.transform.position, 0.1f);

                    isBallOut = false;
                    ball = null;

                    print("testTube5MainColor = " + testTube5MainColor);

                    touchTwice--;

                    changeMainColorOfTestTube();
                }
            }
            else
            {
                Debug.Log("Stack5 is full");
            }
        }
        if (stack5.Count == 4)
        {
            foreach (var item in stack5)
            {
                if (item.tag == testTube5MainColor)
                {
                    fiCount = 1;
                }
                else
                {
                    fiCount = 0;

                    break;
                }
            }

            print("fiCount " + fiCount);

        }
        else
        {
            fiCount = 0;
            print("fiCount " + fiCount);
        }

        if (fiCount == 1)
        {
            done++;
            EmptyTestTubeTouch2.ButtonPressed -= EmptyTestTubeTouch2ButtonPressed;
        }
        print("Testtube5 done " + done);
    }

    void changeMainColorOfTestTube()
    {
        if (stack1.Count!=0 )
        {
            tempBall = stack1.Peek();
            testTube1MainColor = tempBall.tag;
            print("Now testTube1MainColor = " + testTube1MainColor);
        }
        else
        {
            testTube1MainColor = "";
            print("Empty testTube1MainColor = " + testTube1MainColor);
        }

        if(stack2.Count!=0)
        {
            tempBall = stack2.Peek();
            testTube2MainColor = tempBall.tag;
            print("Now testTube2MainColor = " + testTube2MainColor);
        }
        else
        {
            testTube2MainColor = "";
            print("Empty testTube2MainColor = " + testTube2MainColor);
        }

        if (stack3.Count != 0)
        {
            tempBall = stack3.Peek();
            testTube3MainColor = tempBall.tag;
            print("Now testTube3MainColor = " + testTube3MainColor);

        }
        else
        {
            testTube3MainColor = "";
            print("Empty testTube3MainColor = " + testTube3MainColor);
        }


        if (stack4.Count != 0)
        {
            tempBall = stack4.Peek();
            testTube4MainColor = tempBall.tag;
            print("Now testTube4MainColor = " + testTube4MainColor);

        }
        else
        {
            testTube4MainColor = "";
            print("Empty testTube4MainColor = " + testTube4MainColor);
        }


        if (stack5.Count != 0)
        {
            tempBall = stack5.Peek();
            testTube5MainColor = tempBall.tag;
            print("Now testTube5MainColor = " + testTube5MainColor);
        }
        else
        {
            testTube5MainColor = "";
            print("Empty testTube5MainColor = " + testTube5MainColor);
        }

        tempBall = null;
}


    private void OnDestroy()
    {
        TestTubeTouch1.ButtonPressed -= TestTubeTouch1ButtonPressed;
        TestTubeTouch2.ButtonPressed -= TestTubeTouch2ButtonPressed;
        TestTubeTouch3.ButtonPressed -= TestTubeTouch3ButtonPressed;

        EmptyTestTubeTouch1.ButtonPressed -= EmptyTestTubeTouch1ButtonPressed;
        EmptyTestTubeTouch2.ButtonPressed -= EmptyTestTubeTouch2ButtonPressed;
    }

}

[System.Serializable]
public class balls
{
    public string tag;
    public string index;
    public GameObject ballObject;
}