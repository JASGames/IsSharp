using System;
using Xunit;

namespace IsSharp.Test
{
    public class GuardGreaterThanToFixture
    {
        const decimal ConstantValue = 15m;

        [Fact]
        public void CanPrintAFriendExceptionMessage()
        {
            var CostPrice = 15m;
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => CostPrice > 25m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "CostPrice : 15 should be greater than 25");
        }

        [Fact]
        public void CanPrintAFriendExceptionMessageWithCustomerException()
        {
            decimal? CostPrice = 15m;
            var ex = Assert.Throws<MyException>(() => Guard.Is<MyException>(() => CostPrice > 25m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "CostPrice : 15 should be greater than 25");
        }

        [Fact]
        public void ConstantsWillOptimised()
        {
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => ConstantValue > 25m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "False");
        }
    }
}