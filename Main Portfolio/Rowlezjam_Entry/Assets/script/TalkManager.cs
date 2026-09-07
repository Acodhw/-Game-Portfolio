using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    // Start is called before the first frame update
    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData() {
        talkData.Add(1, new string[] {
            "Elementalist$I sense Fire Element pulses nearby. Gotta head there first." });
        talkData.Add(2, new string[] {
             "Elementalist$I\'m aware there\'s goblin territory nearby.",
             "Elementalist$Might be wise to tread carefully around these parts."
        });
        talkData.Add(3, new string[] {
             "Goblins$Intruder spotted! Let's teach \'em a lesson!",
             "Elementalist$Seems I've been discovered.",
             "Elementalist$I can sense the fire element just beyond this point.",
             "Elementalist$No choice but to press forward.",
        });
        talkData.Add(4, new string[] {
             "Elementalist$I\'m drawing closer to the Fire Element.",
             "Elementalist$With traps scattered about, I'll need to watch my step carefully.",
        });
        talkData.Add(5, new string[] {
             "Elementalist$This place... could it be the Temple of Fire?",
             "Elementalist$Why would the stolen element be in a temple?",
             "Elementalist$Regardless, I'll need to enter if I'm to reclaim the element."
        });
        talkData.Add(6, new string[] {
             "Elementalist$The alarm system in the Fire Temple is active.",
             "Elementalist$Most likely triggered by whoever hid the element here.",
             "Elementalist$I\'d better watch out for traps."
        });
        talkData.Add(7, new string[] {
             "Fire Priest$How... how did you make it here?",
             "Fire Priest$Without the element, entry should have been impossible!",
             "Elementalist$You\'re... the Fire Priest? A servant of the gods, stealing an element?",
             "Fire Priest$Bearer of divine mandate, are you blind to the gods' intentions?",
             "Elementalist$The gods\' intentions?",
             "Fire Priest$As priests,",
             "Fire Priest$we\'ve uncovered the malevolent intentions of the elemental deities.",
             "Fire Priest$We shall never relinquish the element!"
        });
        talkData.Add(8, new string[] {
             "Elementalist$The water element is next, it seems.",
             "Elementalist$I can sense its power beyond this desert.",
             "Elementalist$If I recall correctly,",
             "Elementalist$If there\'s a Water Temple somewhere in that area.",
             "Elementalist$I should make my way there.",
             "Elementalist$Fortunately, I can now harness the power of the Fire Element.",
             "Elementalist$I\'ll need to put it to good use."
        });
        talkData.Add(9, new string[] {
             "Elementalist$If I need to enter the Water Temple,",
             "Elementalist$I might need an object containing the water element again.",
             "Elementalist$Perhaps I should look for a water flask before moving on.",
        });
        talkData.Add(10, new string[] {
             "Elementalist$As I thought, I can sense the water element here.",
             "Elementalist$And once again, the traps are active. ",
             "Elementalist$What could be the priests' reason for hiding the elements?",
        });
        talkData.Add(11, new string[] {
             "Water Priest$So you\'ve come this far after defeating the Fire Priest?",
             "Elementalist$It seems clear that the four priests are the culprits.",
             "Elementalist$Why did you steal the elements?",
             "Water Priest$It\'s because the current divine revelation is wrong!",
             "Water Priest$He aims to destroy the very source of our world!",
             "Elementalist$What?",
             "Water Priest$That\'s why, we absolutely cannot relinquish this water element!",
        });
        talkData.Add(12, new string[] {
             "Elementalist$Next, I need to find the Earth Element.",
             "Elementalist$It was located along the beach in the Water Temple.",
             "Elementalist$They might have secured it again,",
             "Elementalist$so I\'ll also need to locate the Earth Elemental artifact.",
             "Elementalist$There\'s probably a rare Earth Mushroom growing around here.",
             "Elementalist$Fortunately, I can now harness the power of the fire element.",
             "Elementalist$I should look for this before moving on."
        });
        talkData.Add(13, new string[] {
             "Elementalist$Come to think of it,",
             "Elementalist$Earth Mushrooms are one of the goblins' favorite foods.",
             "Elementalist$They might have secured it again,",
             "Elementalist$It\'s likely that they\'ll be found where goblins are gathered.",
        });
        talkData.Add(14, new string[] {
             "Elementalist$The traps in the Earth Temple are active too. ",
             "Elementalist$One wrong move and I could fall to the hole",
             "Elementalist$I\'d better tread carefully.",
        });
        talkData.Add(15, new string[] {
             "Earth Priest$Is it down to me and the wind element now?",
             "Elementalist$Well then, will you return the element?",
             "Earth Priest$No, I won't aid in the ruinous future the gods desire!",
             "Elementalist$Ruinous future?",
             "Earth Priest$Are you aware of the immense power",
             "Earth Priest$that can be wielded when all four elements are united?",
             "Elementalist$Of course I am.",
             "Earth Priest$Then, have you not considered",
             "Earth Priest$that if all four elements were to come together,",
             "Earth Priest$it might be possible to kill a god?",
             "Elementalist$Kill... a god?",
             "Earth Priest$Yes, that is the revelation we received.",
             "Earth Priest$If that comes to pass, this world will fall into utter chaos.",
             "Earth Priest$That's why we can never surrender the elements!",
        });
        talkData.Add(16, new string[] {
             "Elementalist$Only the wind element remains now.",
             "Elementalist$It\'s likely in the Wind Temple.",
             "Elementalist$Bird feathers sometimes possess wind attributes,",
             "Elementalist$so I should look for those.",
             "Elementalist$It might be found where the wind appears.",
             "Elementalist$And I need to ask the deity about this revelation too.",
        });
        talkData.Add(17, new string[] {
             "Elementalist$The last one. Let\'s go!",
        });

        talkData.Add(18, new string[] {
             "Wind Priest$Ah... It seems this is the end of the road.",
             "Elementalist$Before taking the element, let me ask one thing.",
             "Elementalist$What does the deity of the elements intend to do?",
             "Wind Priest$Do you understand that losing the deity of the elements",
             "Wind Priest$means the destruction of all four elements?",
             "Elementalist$Is the deity¡¯s intention... to destroy the elements?",
             "Wind Priest$Yes.",
             "Elementalist$In that case, I¡¯ll take the last element",
             "Elementalist$and ask the deity of the elements directly.",
             "Wind Priest$I do not wish to see this world of elements fall apart.",
             "Wind Priest$None of us do.",
             "Wind Priest$Therefore, I will stand against you.",
        });
        talkData.Add(19, new string[] {
             "Elementalist$I¡¯ve gathered all the elements and returned.",
             "Elementalist$Now, it¡¯s time to ask.",
        });
        talkData.Add(20, new string[] {
             "God$Now, the final battle is upon us.",
             "God$ Fight with all thy might!",
        });
        talkData.Add(21, new string[] {
             "God$This level of power cannot yet deplete my strength. ",
             "God$ I shall unleash my true force!",
        });


        talkData.Add(101, new string[] {
            "Sign$CAUTION: SLIMES INFASTATION AREA" });
        talkData.Add(102, new string[] {
            "Sign$WARNING: FALLING HAZARD" });
        talkData.Add(103, new string[] {
            "message$Oi! This 'ere be our turf!" });
        talkData.Add(104, new string[] {
            "message$Watch out for traps during trainin\'! They hurt bad, they do!" });
        talkData.Add(105, new string[] {
            "Sign$Ahead lies the Temple of Fire" });
        talkData.Add(106, new string[] {
            "Sign$Those who seek passage, reveal the element of fire",
            "Elementalist$At this moment, I lack the power of the Fire Element.",
            "Elementalist$So, where could I possibly find an object imbued with the element of fire?",
            "Elementalist$Once I have it, I¡¯ll throw it at that door and see what happens.",});
        talkData.Add(107, new string[] {
            "Sign$CAUTION: LAVA" });
        talkData.Add(108, new string[] {
            "Sign$Warning: Beware of sharp objects including cacti and scorpions" });
        talkData.Add(109, new string[] {
            "Sign$The power of the fire element may allow passage through lava" });
        talkData.Add(110, new string[] {
            "Sign$Caution: Golem Sightings" });
        talkData.Add(111, new string[] {
            "Sign$Ahead lies the Temple of Water" });
        talkData.Add(112, new string[] {
            "Sign$Those who seek passage, reveal the element of water",});
        talkData.Add(113, new string[] {
            "Sign$According to rumors, those who wield the Water Element are said to walk on water",});
        talkData.Add(114, new string[] {
            "Sign$Ahead lies the Temple of Earth" });
        talkData.Add(115, new string[] {
            "Sign$Those who seek passage, reveal the element of earth",});
        talkData.Add(116, new string[] {
            "Sign$Beware of Falling from the Mountain",});
        talkData.Add(117, new string[] {
            "Sign$Ahead lies the Temple of Wind" });

    }

    public string[] ReturnTalkData(int id) {
        return talkData[id];
    }
}
