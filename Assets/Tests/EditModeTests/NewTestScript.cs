using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class NewTestScript
{
    private SlotCalculator slotCalculator;

    private SlotConfig slotConfig;
    private SymbolManager symbolManager;
    private SlotGridTestCase[] testCases;
    private SlotGrid slotGrid;

    [SetUp]
    public void SetUp()
    {
        slotConfig = Resources.Load<SlotConfig>("Tests/TestSlot");

        Assert.NotNull(slotConfig, "Failed to load SlotConfig ScriptableObject.");
        symbolManager = new SymbolManager(slotConfig.symbols);
        slotCalculator = new SlotCalculator();
        testCases = Resources.LoadAll<SlotGridTestCase>("Tests/SlotTestCases");
        Assert.IsNotNull(testCases, "Failed to load test cases.");

    }

    /*// A Test behaves as an ordinary method
    [Test]
    public void FullBoardSameSymbols()
    {
        slotGrid.SetGridTest();
        Assert.AreEqual(s, 33);
    }*/

    [Test]
    public void TestPredefinedGrids()
    {

        foreach (var testCase in testCases)
        {
            ApplyTestCase(testCase);
            int result = slotCalculator.CalculateWin(slotGrid, 1, slotConfig);
            Assert.AreEqual(testCase.expectedResult, result, $"Mismatch in {testCase.name}");
        }
    }

    private void ApplyTestCase(SlotGridTestCase testCase)
    {
        int rows = testCase.grid.Count;
        int columns = testCase.grid[0].rowValues.Count;
        slotGrid = new SlotGrid(rows, columns, new List<CharacterStat>(columns) , symbolManager);
        int[, ]  grid = new int[rows, columns];
        
        //slotGrid.SetGridTest(testCase);
        
    }
}
