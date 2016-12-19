using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for updating the date label.
/// Uses the timescale from the stellar system.
/// Starts counting from January 1st at 12pm of the base year.
/// Accounts for leap year.
/// </summary>
public class DateCalc : MonoBehaviour
{
    private Text _dateLabel;
    private float _timeScale;
    private float _accumulatedTime = 0.5f;  // 0.5 as we start at 12pm, but could be another time.
    private int _baseYear = 2000;

    private void Start()
    {
        FindObjectOfType<StellarSystem>().Scaling += UpdateScale;
        _timeScale = FindObjectOfType<StellarSystem>().TimeScale;
        _dateLabel = GetComponent<Text>();
    }

    private void Update()
    {
        _accumulatedTime += Time.deltaTime * _timeScale;
        UpdateDateLabel();
    }

    /// <summary>
    /// Updates the time scale from StellarSystem.
    /// </summary>
    /// <param name="variable">Which scale.</param>
    private void UpdateScale(string variable, float value)
    {
        if (variable == "time")
            _timeScale = value;
    }

    /// <summary>
    /// Updates the date Label based on the current accumulated time and the base year and day time.
    /// The idea is to take the current accumulated time as a pool.
    /// We calculate the year then month, each time substracting the numer of days from the pool.
    /// </summary>
    private void UpdateDateLabel()
    {
        // The timePool to substract from.
        int timePool = Mathf.FloorToInt(_accumulatedTime);

        int year = ComputeYear(ref timePool, _baseYear);
        int month = ComputeMonth(ref timePool, IsLeapYear(year));
        int day = timePool + 1; // +1 as we start at 0

        // Convert to string and add 0 in front of day and month to keep the same structure.
        string dd = day.ToString();
        string mm = month.ToString();
        string yyyy = year.ToString();

        if (day.ToString().Length == 1)
            dd = "0" + day;
        if (month.ToString().Length == 1)
            mm = "0" + month;

        _dateLabel.text = dd + "/" + mm + "/" + yyyy;
    }

    /// <summary>
    /// Computes the year.
    /// </summary>
    /// <returns>The year.</returns>
    /// <param name="pool">The current time pool passed by reference to directly act on it.</param>
    private int ComputeYear(ref int pool, int baseYear)
    {
        // could do a leap check on base year to have a +1 or +0 and use any base year
        int year;
        int dayCheck;
        int leapCheck = (IsLeapYear(baseYear)) ? 1 : 0; // we check if the base year is a leap year

        // at first we ignore the base year and only have the years passed since program start
        year = Mathf.FloorToInt(pool / 365.2425f);

        // the formula below accounts for leap year. The "-1" is due to thought we are in a leap year, we haven't yet used the extra day
        dayCheck = pool - (365 * year + (year - 1) / 4 - (year - 1) / 100 + (year - 1) / 400 + leapCheck);
        if (dayCheck < 0) // due to diff between real year and calendar year, we check that we haven't yet added a year to many
            year -= 1;

        if (year <= 0) // we make sure we don't have a negative year either
            year = 0; // and no need to remove any days as it would be the year
        else // otherwise we remove the number of days corresponding to the number of passed year to the pool
            pool -= (365 * year + (year - 1) / 4 - (year - 1) / 100 + (year - 1) / 400 + leapCheck);

        // finally we return the actual total year
        return year + baseYear;
    }

    /// <summary>
    /// Determines whether the given year is a leap year.
    /// </summary>
    /// <returns><c>true</c> if this instance is leap year; otherwise, <c>false</c>.</returns>
    /// <param name="year">The current total year.</param>
    private bool IsLeapYear(int year)
    {
        if (year % 400 == 0)
            return true;
        else if (year % 100 == 0)
            return false;
        else if (year % 4 == 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Computes the month.
    /// </summary>
    /// <returns>The month.</returns>
    /// <param name="pool">The current time pool passed by reference to directly act on it.</param>
    private int ComputeMonth(ref int pool, bool leap)
    {
        // list of number of accumulated days in the year.
        // 0 for i = 0 to match the actual month number and allow for checking the previous month without getting a negative index
        int[] daysToMonthEnd = new int[] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        int[] daysToMonthEndLeap = new int[] { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
        int month;

        // we loop through the list until the pool is under the number of the given month index and return index as month
        for (month = 1; month <= 12; month += 1)
        {
            if (!leap && pool < daysToMonthEnd[month])
            {
                // we retract the number of days from the previous month (0 for January)
                pool -= daysToMonthEnd[month - 1];
                break;
            }
            else if (leap && pool < daysToMonthEndLeap[month])
            {  // if we are in leap and not january, we check for the month list + 1 day for leap year
                pool -= (daysToMonthEndLeap[month - 1]);
                break;
            }
        }

        return month;
    }
}