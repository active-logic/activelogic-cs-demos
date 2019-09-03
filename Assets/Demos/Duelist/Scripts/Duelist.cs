using UnityEngine;
using Active.Core;
using static Active.Core.status;
using Auto = System.Runtime.CompilerServices.CallerMemberNameAttribute;

// TODO - On approach log-trace is incorrect, with repeated elements.
public class Duelist : UTask{

#if !AL_BEST_PERF
    const string WALK = "Walk", RUN = "Run", IDLE = "Idle";
    const float rotationalSpeed = 180;
    //
    public string    targetName;
    public float     speed = 1f, walkRate = 0.3f;
    public int       health = 100, power = 25, lowHealth = 50, crtHealth = 25,
                     armor = 10;
    public AudioClip footstep, clash, fall;
    //
    float     lastBlock;
    string    currentAnim;
    Duelist   target;
    AudioClip currentSound;

    override public status Step() => (target = FindTarget()) ?
        Eval( Attack() || Defend() || Retreat() ) : Play(IDLE).now;

    status Attack() => health < lowHealth ?
        fail(log && "Low health") : Eval( Approach() && Strike() );

    status Defend() => health < crtHealth ?
        fail(log && "Critical health") : Eval( Await() && Block().now );

    status Retreat() => Eval(Move(transform.position - battle, RUN, 2.5f));

    // Deps 1 -----------------------------------------------------------------

    status Approach() => dist <= 1f ? done(log && $"Target {target} reached")
        : Eval( Move(target.transform.position - transform.position, WALK, 1) );

    status Strike() => Eval(
        Play().now && After(0.75f)?[ Sound(clash).now && target.Damage(power) ]
    );

    status Await() => Do(transform.forward = dir).now
        && (dist > 1 ? cont() : done(log && $"Attacker {target} in range"));

    action Block() => Do( lastBlock = Time.time ) % Play();

    // Deps 2 -----------------------------------------------------------------

    status Move(Vector3 u, string anim, float speedFactor){
        var t = transform;
        var a = Vector3.SignedAngle(t.forward, u, Vector3.up);
        var A = Mathf.Abs(a);
        var amount = Mathf.Min(Time.deltaTime * rotationalSpeed, A);
        var b = (a > 0 ? 1 : -1) * amount;
        t.RotateAround(t.position, Vector3.up, b);
        if(A < 10){
            t.position += u.normalized * Time.deltaTime * speed * speedFactor;
        }
        return cont() % Every(walkRate)?[ Sound(footstep).now ] % Play(anim).now;
    }

    status Damage(int power){
        power = power-armor; if(power < 0) return done(log && "No damage");
        if(suspended)
            return fail(log && "Already KO");
        else if((health -= power / defence) > 0)
            return done(log && $"Hit - {health}hp remaining");
        else
            return ( Play("KO") % Sound(fall) % Suspend() ).now;
    }

    Duelist FindTarget(){
        Duelist[] all = FindObjectsOfType<Duelist>();
        Duelist sel = null;
        float d0 = 0;
        foreach(Duelist k in all){
            if(k == this || k.suspended || Faction(k) == Faction(this))
                continue;
            if(sel == null || Dist(k) < d0){
                sel = k;
                d0 = Dist(k);
            }
        }
        return sel;
    }

    float Dist(Duelist x)
    => (x.transform.position - transform.position).magnitude;

    string Faction(Duelist x) => x.transform.GetChild(0).gameObject.name;

    action Play([Auto] string anim=""){
        if(anim != currentAnim) animator.CrossFade(currentAnim = anim, .1f);
        return @void(log && $"Anim: '{anim}'");
    }

    action Sound(AudioClip clip, bool loop = false){
        if(clip==null) throw new System.ArgumentException("No sound clip");
        AudioSource src = GetComponent<AudioSource>();
        if(!src) src = gameObject.AddComponent<AudioSource>();
        if(clip != currentSound || !src.isPlaying){
            src.loop = loop;
            src.clip = currentSound = clip;
            src.Play();
        }
        return @void(log && $"♫ {clip.name}");
    }

    // ------------------------------------------------------------------------

    int      defence  => (Time.time - lastBlock) < .75f ? 2 : 1;
    float    dist     => dir.magnitude;
    Animator animator => GetComponentInChildren<Animator>();
    Vector3  dir      => target.transform.position - transform.position;

    Vector3  battle{ get{
        Vector3   s   = Vector3.zero;
        Duelist[] all = FindObjectsOfType<Duelist>();
        foreach(Duelist k in all)
            if(k != this) s += k.transform.position;

        return s/all.Length;
    }}
#endif
}
