using System;
using Xunit;

namespace IsSharp.Test
{
    public class GuardIsNullOrWhiteSpaceFixture
    {
        const decimal ConstantValue = 15m;

        [Fact]
        public void CanPrintAFriendExceptionMessage()
        {
            string Name = null;
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => !string.IsNullOrWhiteSpace(Name)));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "Name : null should not be null or whitespace ");
        }

        [Fact]
        public void CanPrintAFriendExceptionMessageWithCustomerException()
        {
            string Name = null;
            var ex = Assert.Throws<MyException>(() => Guard.Is<MyException>(() => !string.IsNullOrWhiteSpace(Name)));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "Name : null should not be null or whitespace ");
        }

        [Fact]
        public void ConstantsWillOptimised()
        {
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => ConstantValue >= 25m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "False");
        }
    }
}