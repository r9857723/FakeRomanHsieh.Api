using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Stateless;

namespace FakeRomanHsieh.API.Models
{
    public class Order
    {
        public Order()
        {
            StateMachineInit();
        }
        [Key]
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public String TranscationMetadata { get; set; }
        StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        public void PaymentPrcoessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PlaceOrder);
        }
        public void PaymenyApprove()
        {
            _machine.Fire(OrderStateTriggerEnum.approve);
        }
        public void PaymentReject()
        {
            _machine.Fire(OrderStateTriggerEnum.Reject);
        }

        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(() => State, s => State = s);

            // 狀態:生成 -> 1.動作:支付訂單=>狀態:處理中 2.動作:取消訂單=>狀態:取消
            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);
            // 狀態:處理中 -> 1.動作:收款成功=>狀態:支付成功  2.動作:收款失敗=>狀態:支付失敗
            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);
            // 狀態:交易失敗 -> 1.動作:支付訂單=>狀態:處理中
            _machine.Configure(OrderStateEnum.Declined)
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);
            // 狀態:交易完成 -> 1.動作:退款=>狀態:退費
            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refuund);
        }
    }

    public enum OrderStateEnum
    {
        Pending,    // 訂單生成
        Processing, // 處理中
        Completed,  // 交易完成
        Declined,   // 交易失敗
        Cancelled,  // 取消
        Refuund     // 退款
    }

    public enum OrderStateTriggerEnum
    {
        PlaceOrder, // 支付
        approve,    // 支付成功
        Reject,     // 支付失敗
        Cancel,     // 取消
        Return      // 退費
    }
}
