using System;
using Xunit;

namespace IsSharp.Test
{
    public class GuardLessThanFixture
    {
        const decimal ConstantValue = 55m;

        [Fact]
        public void CanPrintAFriendExceptionMessage()
        {
            var CostPrice = 55m;
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => CostPrice < 45m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "CostPrice(55) should be less than 45");
        }

        [Fact]
        public void CanPrintAFriendExceptionMessageWithCustomerException()
        {
            decimal? CostPrice = 55m;
            var ex = Assert.Throws<MyException>(() => Guard.Is<MyException>(() => CostPrice < 45m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "CostPrice(55) should be less than 45");
        }

        [Fact]
        public void ConstantsWillOptimised()
        {
            var ex = Assert.Throws<ArgumentException>(() => Guard.Is(() => ConstantValue < 45m));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, "False");
        }
    }
}