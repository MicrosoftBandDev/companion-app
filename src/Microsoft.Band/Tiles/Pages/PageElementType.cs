// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.PageElementType
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    internal enum PageElementType : ushort
    {
        PageHeader = 1,
        FlowListTypesStart = 1000, // 0x03E8
        NonDataTypesStart = 1000, // 0x03E8
        FlowList = 1001, // 0x03E9
        ScrollFlowList = 1002, // 0x03EA
        FlowListTypesEnd = 1100, // 0x044C
        ShapeTypesStart = 1100, // 0x044C
        FilledQuad = 1101, // 0x044D
        ShapeTypesEnd = 1200, // 0x04B0
        DataTypesStart = 3000, // 0x0BB8
        NonDataTypesEnd = 3000, // 0x0BB8
        TextTypesStart = 3000, // 0x0BB8
        Text = 3001, // 0x0BB9
        WrappableText = 3002, // 0x0BBA
        IconTypesStart = 3100, // 0x0C1C
        TextTypesEnd = 3100, // 0x0C1C
        Icon = 3101, // 0x0C1D
        BarcodeTypesStart = 3200, // 0x0C80
        IconTypesEnd = 3200, // 0x0C80
        BarcodeCode39 = 3201, // 0x0C81
        BarcodePdf417 = 3202, // 0x0C82
        BarcodeTyepsEnd = 3300, // 0x0CE4
        InteractiveElementsStart = 3300, // 0x0CE4
        InteractiveButton = 3301, // 0x0CE5
        InteractiveButtonWithBorder = 3302, // 0x0CE6
        InteractiveButtonWithText = 3303, // 0x0CE7
        InteractiveButtonWithIcon = 3304, // 0x0CE8
        InteractiveElementEnd = 3400, // 0x0D48
        DataTypesEnd = 13000, // 0x32C8
        SetColors = 13001, // 0x32C9
        InvalidType = 65535, // 0xFFFF
    }
}
