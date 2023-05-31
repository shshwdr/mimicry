using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AffectableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isAffected = false;

    public GameObject dialoguePanel;
    public TMP_Text dialogueLabel;
    public bool isDestroyed = false;

    public float moveTime = 0.1f;
    //public Button actionButton;
    public SpriteRenderer renderer;
    public bool isDead = false;
    public Direction dir = Direction.right;

    public Transform indicator;

    public bool canMove = false;
    protected Animator animator;
    public Vector2Int pos;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        updateAffectVisual();
        pos = GridManager.PositionToIndexPair(transform.position);
        transform.position = GridManager.indexToPosition(pos);
        GridManager.Instance.AddAffectable(this);
        moveTime = AffectableManager.moveTime;
        //actionButton.onClick.AddListener(action);
    }

    public bool isInFrontOf(AffectableItem item)
    {
        var positionDiff = pos - item.pos;// (pos.x - item.pos.x, pos.y - item.pos.y);
        return item.isInFront(positionDiff.x, positionDiff.y);
        // var ddir = GridManager.directionToPair( item.dir);
        //
        // //add equal to affect only when restrict at the back
        // if (ddir.Item1 * positionDiff.Item1 > 0 || ddir.Item2 * positionDiff.Item2 > 0)
        // {
        //     return true;
        // }
        //
        // return false;
    }

    public virtual IEnumerator enemyChestCheck()
    {
        yield return null;
    }
    public bool isInFront(int x, int y)
    {
        var ddir = GridManager.directionToVector2Int(dir);

        //add equal to affect only when restrict at the back
        // if (ddir.Item1 * x > 0 || ddir.Item2 * y > 0)
        // {
        //     return true;
        // }
        //
        // return false;

        if (ddir.x * x < 0 || ddir.y * y < 0)
        {
            return false;
        }

        return true;
    }

    public virtual void enemyMove()
    {
        
    }

    public GameObject playEffect(string effectName)
    {
        //add blood effect
        var bloodEffectPrefab = Resources.Load<GameObject>(effectName);
        var bloodEffect = Instantiate(bloodEffectPrefab);
        bloodEffect.transform.position = transform.position;
        return bloodEffect;
    }

    public GameObject playEffect(string effectName, Vector3 pos)
    {
        //add blood effect
        var bloodEffectPrefab = Resources.Load<GameObject>(effectName);
        var bloodEffect = Instantiate(bloodEffectPrefab);
        bloodEffect.transform.position = pos;
        return bloodEffect;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void updateAffectVisual()
    {
    }

    public virtual void editorSafe_UpdateAffectVisual(Sprite sprite)
    {
        if (sprite == null && animator != null) //just a normal update
        {
            updateAffectVisual();
        }
        else //probably in the editor. Just update sprite.
        {
            renderer.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }



    // public void Rotate(Direction d)
    // {
    //     if (this is Human human)
    //     {
    //         dir = d;
    //         var dirPair = GridManager.directionToVector2Int(d);
    //         human.updateFaceAnim(dirPair.x, dirPair.y);
    //     }
    // }

    // public virtual IEnumerator RotateEnumerator(Direction d)
    // {
    //     Rotate(d);
    //     yield return null;
    // }
    public virtual IEnumerator DieEnumerator(bool isDie)
    {
        yield return null;
    }

    public virtual void Die(bool isDie)
    {
        isDead = isDie;
        if (!isDie)
        {
            animator.Rebind();
            
        }
        else
        {
            animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("die");
        }
        updateAffectVisual();
    }
    
    //
    // public void DoEffect(EffectCommand command)
    // {
    //     if (command.type == EffectType.MaskBlock){
    //     
    //         AudioManager.Instance.PlayOneShot(FMODEvents.Instance.maskBlock, transform.position);
    //     }
    // }

    public void Move(Vector2Int position)
    {
        pos = position;
        transform.position = GridManager.indexToPosition(pos);
    }

    public virtual IEnumerator MoveEnumerator(Vector2Int position)
    {
        foreach (var item in GridManager.Instance.affectableItems)
        {
            if (item is Enemy enemy && enemy.canSeeChest(false))
            {
                var dir = GridManager.indexToPosition(position) - GridManager.indexToPosition(pos);
                transform.DOMove(GridManager.indexToPosition(pos) + dir*0.3f, moveTime*0.3f);
                yield return StartCoroutine(enemy.catchMimicry());
                yield break;
            }
        }
        
        
        pos = position;
        transform.DOMove(GridManager.indexToPosition(pos), moveTime);
        
        foreach (var item in GridManager.Instance.affectableItems)
        {
            if (item is Enemy enemy && enemy.canSeeChest(false))
            {
                yield return StartCoroutine(enemy.catchMimicry());
                yield break;
            }
        }
        
        
        // AudioManager.Instance.PlayOneShot(FMODEvents.Instance.walk_woosh, transform.position);
        // if (this is Human human)
        // {
        //     human.triggerMove();
        // } 
        
        yield return new WaitForSeconds(moveTime);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        dialoguePanel.SetActive(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        dialoguePanel.SetActive(false);
    }
}