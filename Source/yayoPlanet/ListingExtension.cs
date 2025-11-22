using Verse;
using UnityEngine;

namespace yayoPlanet;

public static class ListingExtensions
{
    public static void IntEntryWithAdjuster(
        this Listing_Standard listing,
        string key,
        ref int value,
        int step,
        int min,
        int max)
    {
        Rect row = listing.GetRect(28f);

        Rect labelRect = row.LeftPart(0.45f);
        Rect minusRect = new Rect(row.x + row.width * 0.48f, row.y, 28f, row.height);
        Rect fieldRect = new Rect(row.x + row.width * 0.48f + 32f, row.y, 80f, row.height);
        Rect plusRect = new Rect(row.x + row.width * 0.48f + 116f, row.y, 28f, row.height);

        // Label
        Widgets.Label(labelRect, key.Translate(value));

        // --- minus button ---
        if (Widgets.ButtonText(minusRect, "-"))
            value = Mathf.Clamp(value - step, min, max);

        // --- numeric entry ---
        string buffer = value.ToString();
        Widgets.TextFieldNumeric(fieldRect, ref value, ref buffer, min, max);

        // --- plus button ---
        if (Widgets.ButtonText(plusRect, "+"))
            value = Mathf.Clamp(value + step, min, max);
    }
}
