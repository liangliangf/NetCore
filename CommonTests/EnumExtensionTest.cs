using MS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CommonTests
{
    public class EnumExtensionTest
    {
        [Fact]
        [Trait("GetEnum", "itemName")]//Trait特性可以对测试用例进行分组说明
        public void GetEnum_EnumName_ReturnCorrespondEnum() //测试方法命名规则为"要测试的方法的名称+测试的方案+调用方案时的预期行为
        {
            //Arrange（安排对象，根据需要对其进行创建和设置）
            StatusCode statusCode = StatusCode.Deleted;

            //Act（作用于对象）
            string actual = statusCode.ToString();

            //Assert（断言某些项按预期进行）
            Assert.Equal(statusCode, actual.GetEnum<StatusCode>());
        }
        [Fact]
        [Trait("GetEnum", "itemValue")]
        public void GetEnum_EnumValue_ReturnCorrespondEnum()
        {
            //Arrange
            StatusCode statusCode = StatusCode.Disable;

            //Act
            int actual = statusCode.GetHashCode();

            //Assert
            Assert.Equal(statusCode, actual.GetEnum<StatusCode>());
        }

        [Fact]
        [Trait("GetEnumName", "itemValue")]
        public void GetEnumName_EnumValue_ReturnCorrespondEnumName()
        {
            //Arrange
            StatusCode statusCode = StatusCode.Enable;

            //Act
            int actual = statusCode.GetHashCode();

            //Assert
            Assert.Equal(statusCode.ToString(), actual.GetEnumName<StatusCode>());
        }

        [Fact]
        [Trait("GetEnumValue", "itemName")]
        public void GetEnumValue_EnumName_ReturnCorrespondEnumValue()
        {
            //Arrange
            StatusCode statusCode = StatusCode.Disable;

            //Act
            string actual = statusCode.ToString();

            //Assert
            Assert.Equal(statusCode.GetHashCode(), actual.GetEnumValue<StatusCode>());
        }

        [Fact]
        [Trait("GetDescription", "Enum")]
        public void GetDescription_Enum_ReturnCorrespondEnumDescription()
        {
            //Arrange
            StatusCode statusCode = StatusCode.Deleted;

            //Assert
            Assert.Equal("已删除", statusCode.GetDescription());
        }


    }
}
